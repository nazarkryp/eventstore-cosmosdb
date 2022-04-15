using System.Text.Json;

using NKryp.EventStore.CosmosDb.Abstractions;
using NKryp.EventStore.CosmosDb.Documents;
using NKryp.EventStore.CosmosDb.Exceptions;

namespace NKryp.EventStore.CosmosDb.Serialization
{
    internal interface IDocumentSerializer
    {
        EventData DeserializeEvent(EventDocument document);

        EventDocument SerializeEvent(EventData @event, string streamId);

        object? DeserializeObject(string typeName, object data);
    }

    internal class EventDocumentSerializer : IDocumentSerializer
    {
        private readonly ITypeProvider _typeProvider;
        private readonly bool _ignoreMissingTypes;

        public EventDocumentSerializer(
            ITypeProvider typeProvider)
        {
            _typeProvider = typeProvider;
            _ignoreMissingTypes = false;
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
                BodyType = _typeProvider.GetEventId(@event.Body!.GetType()),
                Body = @event.Body
            };

            return document;
        }

        public object? DeserializeObject(string typeName, object data)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            var type = _typeProvider.GetType(typeName);

            if (type == null)
            {
                if (_ignoreMissingTypes)
                {
                    return null;
                }

                throw new TypeNotFoundException(typeName);
            }

            try
            {
                return ((JsonElement)data).Deserialize(type);
            }
            catch (Exception ex)
            {
                throw new JsonDeserializationException(typeName, data.ToString(), ex);
            }
        }
    }
}
