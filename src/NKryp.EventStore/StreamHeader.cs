namespace NKryp.EventStore
{
    public class StreamHeader
    {
        public StreamHeader(string streamId, ulong version)
        {
            StreamId = streamId;
            Version = version;
        }

        public string StreamId { get; }

        public ulong Version { get; }
    }
}
