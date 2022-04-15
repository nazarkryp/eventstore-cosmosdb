namespace NKryp.EventStore.CosmosDb.Exceptions
{
    [Serializable]
    public class StreamNotFoundException : Exception
    {
        public StreamNotFoundException(string streamId)
            : base($"Stream not found. StreamId: {streamId}")
        {
            StreamId = streamId;
        }

        public string StreamId { get; }
    }
}
