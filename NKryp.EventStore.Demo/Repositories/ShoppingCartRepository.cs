using NKryp.EventStore.CosmosDb;
using NKryp.EventStore.CosmosDb.Abstractions;
using NKryp.EventStore.Demo.Domain;

namespace NKryp.EventStore.Demo.Repositories
{
    internal class ShoppingCartRepository
    {
        private readonly IEventStore _eventStore;

        public ShoppingCartRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<ShoppingCartAggregateRoot?> FindAsync(string shoppingCartId)
        {
            var stream = await _eventStore.ReadStreamAsync(shoppingCartId);

            if (stream == null)
            {
                return null;
            }

            return ShoppingCartAggregateRoot.Aggregate(stream.Events.Select(e => (Event)e.Body!).ToArray());
        }

        public async Task SaveAsync(ShoppingCartAggregateRoot aggregateRoot)
        {
            //var v = await _eventStore.GetStreamHeaderAsync(aggregateRoot.Id);
            var existingVersion = await GetStreamVersionAsync(aggregateRoot.Id);
            var version = existingVersion.GetValueOrDefault() + 1;

            List<EventData> events = new();

            foreach (var @event in aggregateRoot.GetEvents().Where(e => !e.Processed))
            {
                @event.Processed = true;

                var eventData = new EventData(aggregateRoot.Id, @event, version++);
                events.Add(eventData);
            }

            //var events = aggregateRoot.GetEvents().Select(@event => new EventData(
            //    aggregateRoot.Id,
            //    @event,
            //    version++)).ToArray();

            await _eventStore.AppendStreamAsync(aggregateRoot.Id, events.ToArray(), existingVersion);
        }

        private async Task<ulong?> GetStreamVersionAsync(string streamId)
        {
            ulong? version = null;

            var query = $"SELECT TOP 1 * from c WHERE c.StreamId = '{streamId}'";

            await _eventStore.LoadStreamHeaders(query, headers =>
            {
                if (headers.Count > 0)
                {
                    version = headers.First().Version;
                }

                return Task.CompletedTask;
            });

            return version;
        }
    }
}
