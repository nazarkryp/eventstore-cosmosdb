using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

using NKryp.EventStore.CosmosDb.Abstractions;
using NKryp.EventStore.CosmosDb.Documents;
using NKryp.EventStore.CosmosDb.Exceptions;
using NKryp.EventStore.CosmosDb.Extensions;
using NKryp.EventStore.CosmosDb.Options;
using NKryp.EventStore.CosmosDb.Persistence;
using NKryp.EventStore.CosmosDb.Serialization;

namespace NKryp.EventStore.CosmosDb
{
    internal class EventStore : IEventStore
    {
        private readonly IContainerFactory _containerFactory;
        private readonly IDocumentSerializer _serializer;

        private Container? _container;

        public EventStore(
            IContainerFactory containerFactory,
            IDocumentSerializer serializer,
            IOptions<EventStoreOptions> options)
        {
            QueryMaxItemCount = options.Value.QueryMaxItemCount;

            _containerFactory = containerFactory;
            _serializer = serializer;
        }

        public byte BatchSize { get; set; } = 100;

        public int QueryMaxItemCount { get; set; }

        public Container Container => _container ??= _containerFactory.GetContainer();

        public async Task<StreamHeader?> GetStreamHeaderAsync(string streamId)
        {
            try
            {
                var result = await Container.ReadItemAsync<EventDocument>(streamId, new PartitionKey(streamId));

                return new StreamHeader(streamId, result.Resource.Version);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public Task LoadStreamHeaders(string query, Func<IReadOnlyCollection<StreamHeader>, Task> callback)
            => LoadDocuments(
                new QueryDefinition(query),
                response => callback(
                    response
                        .Where(x => x.DocumentType == DocumentType.Header)
                        .Select(x => new StreamHeader(x.StreamId, x.Version))
                        .ToList()));

        public Task<EventStream?> ReadStreamAsync(string streamId)
        {
            var options = new ReadStreamOptions();

            var maxItemCount = options.MaxItemCount ?? QueryMaxItemCount;

            var whereTerms = new List<string>();

            if (options.FromVersion.HasValue)
            {
                whereTerms.Add($"(x.{nameof(EventDocument.Version)} >= {options.FromVersion.Value} OR x.{nameof(EventDocument.DocumentType)} = '{nameof(DocumentType.Header)}')");
            }

            if (options.ToVersion.HasValue)
            {
                whereTerms.Add($"(x.{nameof(EventDocument.Version)} <= {options.ToVersion.Value} OR x.{nameof(EventDocument.DocumentType)} = '{nameof(DocumentType.Header)}')");
            }

            var selectClause = "SELECT * FROM x";
            var whereClause = whereTerms.Count > 0 ? $"WHERE {string.Join(" AND ", whereTerms)}" : string.Empty;
            var orderByClause = $"ORDER BY x.{nameof(EventDocument.SortOrder)} DESC";

            var query = $"{selectClause} {whereClause} {orderByClause}";

            return ReadStream(streamId, query, maxItemCount);
        }

        public async Task AppendStreamAsync(string streamId, EventData[] events, ulong? expectedVersion = null)
        {
            var transaction = Container.CreateTransactionalBatch(new PartitionKey(streamId));

            if (expectedVersion.HasValue)
            {
                var header = await ReadHeaderDocument(streamId);

                if (header.Version != expectedVersion)
                {
                    throw new ConcurrencyException(streamId, expectedVersion.Value, header.Version);
                }

                header.Version += (ulong)events.Length;

                transaction.ReplaceItem(header.Id, header, new TransactionalBatchItemRequestOptions { IfMatchEtag = header.ETag });
            }
            else
            {
                var header = new EventDocument(DocumentType.Header)
                {
                    StreamId = streamId,
                    Version = (ulong)events.Length
                };

                transaction.CreateItem(header);
            }

            var firstBatch = events
                .Take(BatchSize - 1)
                .Select(@event => _serializer.SerializeEvent(@event, streamId))
                .ToArray();

            foreach (var document in firstBatch)
            {
                transaction.CreateItem(document);
            }

            var response = await transaction.ExecuteAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                throw new StreamAlreadyExistsException(streamId);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new StreamWriteException(streamId, response.ErrorMessage, response.StatusCode);
            }

            foreach (var batch in events.Skip(BatchSize - 1).Select(@event => _serializer.SerializeEvent(@event, streamId)).Batch(BatchSize))
            {
                transaction = Container.CreateTransactionalBatch(new PartitionKey(streamId));

                foreach (var document in batch)
                {
                    transaction.CreateItem(document);
                }

                response = await transaction.ExecuteAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new StreamWriteException(streamId, response.ErrorMessage, response.StatusCode);
                }
            }
        }

        private async Task<EventDocument> ReadHeaderDocument(string streamId)
        {
            try
            {
                var result = await Container.ReadItemAsync<EventDocument>(streamId, new PartitionKey(streamId));

                return result.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new StreamNotFoundException(streamId);
            }
        }

        private async Task LoadDocuments(QueryDefinition query, Func<FeedResponse<EventDocument>, Task> callback)
        {
            var iterator = Container.GetItemQueryIterator<EventDocument>(query, requestOptions: new QueryRequestOptions { MaxItemCount = QueryMaxItemCount });
            var callbackProcessing = Task.CompletedTask;

            do
            {
                var response = await iterator.ReadNextAsync();

                await callbackProcessing;

                callbackProcessing = callback(response);
            }
            while (iterator.HasMoreResults);

            await callbackProcessing;
        }

        private async Task<EventStream?> ReadStream(string streamId, string query, int maxItemCount)
        {
            if (streamId == null)
            {
                throw new ArgumentNullException(nameof(streamId));
            }

            var iterator = Container.GetItemQueryIterator<EventDocument>(
                query,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(streamId),
                    MaxItemCount = maxItemCount
                });

            var documents = new List<EventDocument>();

            while (iterator.HasMoreResults)
            {
                var page = await iterator.ReadNextAsync();

                documents.AddRange(page);
            }

            if (documents.Count == 0)
            {
                return null;
            }

            var headerDocument = documents.First(x => x.DocumentType == DocumentType.Header);

            try
            {
                var events = documents.Where(x => x.DocumentType == DocumentType.Event).Select(_serializer.DeserializeEvent).Reverse().ToArray();

                return new EventStream(streamId, headerDocument.Version, events);
            }
            catch (TypeNotFoundException ex)
            {
                throw;
                //throw new StreamDeserializationException(streamId, requestCharge, ex.Type, ex);
            }
            catch (JsonDeserializationException ex)
            {
                throw;
                //throw new StreamDeserializationException(streamId, requestCharge, ex.Type, ex);
            }
        }
    }
}
