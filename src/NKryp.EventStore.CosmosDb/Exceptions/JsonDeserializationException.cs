using System;

namespace NKryp.EventStore.CosmosDb.Exceptions
{
    [Serializable]
    public class JsonDeserializationException : Exception
    {
        public JsonDeserializationException(string type, string json, Exception innerException)
            : base($"Failed to deserialize an instance of '{type}'", innerException)
        {
            Type = type;
            Json = json;
        }

        protected JsonDeserializationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public string? Type
        {
            get => (string?)Data[nameof(Type)];
            private set => Data[nameof(Type)] = value;
        }

        public string? Json
        {
            get => (string?)Data[nameof(Json)];
            private set => Data[nameof(Json)] = value;
        }
    }
}
