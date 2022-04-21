using System;
using System.Net;

namespace NKryp.EventStore.CosmosDb.Exceptions
{
    [Serializable]
    internal class StreamWriteException : Exception
    {
        public StreamWriteException(string streamId, string? message, HttpStatusCode code)
        : base(message ?? $"{nameof(StreamWriteException)} occurred while writing to stream")
        {
            StreamId = streamId;
            Code = code;
        }

        public string StreamId { get; }

        public HttpStatusCode Code { get; }
    }
}
