using System;

namespace NKryp.EventStore.CosmosDb.Exceptions
{
    [Serializable]
    internal class StreamAlreadyExistsException : Exception
    {
        public StreamAlreadyExistsException(string streamId) 
            => StreamId = streamId;

        public string StreamId { get; }
    }
}
