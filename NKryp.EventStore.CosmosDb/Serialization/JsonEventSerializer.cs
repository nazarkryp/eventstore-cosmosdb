using System.Text.Json;

using NKryp.EventStore.CosmosDb.Abstractions;

namespace NKryp.EventStore.CosmosDb.Serialization
{
    internal class JsonEventSerializer : IEventSerializer
    {
        private readonly ITypeProvider _typeProvider;

        public JsonEventSerializer(ITypeProvider typeProvider)
        {
            _typeProvider = typeProvider;
        }

        public T Deserialize<T>(EventData eventData) where T : class
        {
            return null;
            //_typeProvider.GetType(eventData.)
            //var json = (string)source;
            //JsonSerializer.Deserialize(json, )
        }
    }
}
