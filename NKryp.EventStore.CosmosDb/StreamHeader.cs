namespace NKryp.EventStore.CosmosDb
{
    public class StreamHeader
    {
        internal StreamHeader(string streamId, ulong version)
        {
            StreamId = streamId;
            Version = version;
        }

        public string StreamId { get; }

        public ulong Version { get; }
    }
}
