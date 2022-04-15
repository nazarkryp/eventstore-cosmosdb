namespace NKryp.EventStore.CosmosDb
{
    public class EventStream
    {
        public EventStream(string streamId, ulong version, EventData[] events)
        {
            StreamId = streamId;
            Version = version;
            Events = events;
        }

        public string StreamId { get; }

        public ulong Version { get; }

        public EventData[] Events { get; }
    }
}
