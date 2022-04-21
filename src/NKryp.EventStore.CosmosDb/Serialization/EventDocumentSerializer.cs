using System;
using System.Text.Json;

using NKryp.EventStore.Abstractions;
using NKryp.EventStore.CosmosDb.Documents;
using NKryp.EventStore.CosmosDb.Exceptions;

namespace NKryp.EventStore.CosmosDb.Serialization
{
    internal interface IDocumentSerializer
    {
        EventData DeserializeEvent(EventDocument document);

        EventDocument SerializeEvent(EventData @event, string streamId);
    }

    internal class EventDocumentSerializer : IDocumentSerializer
    {
        private readonly ITypeProvider _typeProvider;

        public EventDocumentSerializer(
            ITypeProvider typeProvider)
        {
            _typeProvider = typeProvider;
        }

        public EventData DeserializeEvent(EventDocument document)
        {
            var body = DeserializeObject(document.BodyType, document.Body);

            return new EventData(document.StreamId, body, document.Version);
        }

        public EventDocument SerializeEvent(EventData @event, string streamId)
        {
            var document = new EventDocument(DocumentType.Event)
            {
                StreamId = streamId,
                Version = @event.Version,
                BodyType = _typeProvider.GetEventTypeId(@event.Body!.GetType()),
                Body = @event.Body
            };

            return document;
        }

        private object? DeserializeObject(string typeName, object data)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            var type = _typeProvider.GetType(typeName);

            if (type == null)
            {
                throw new TypeNotFoundException(typeName);
            }

            try
            {
                return JsonSerializer.Deserialize(data.ToString(), type);
            }
            catch (Exception ex)
            {
                throw new JsonDeserializationException(typeName, data.ToString(), ex);
            }
        }
    }
}
