namespace NKryp.EventStore.CosmosDb
{
    public class EventData
    {
        public EventData(string streamId, object? body, ulong version)
        {
            StreamId = streamId;
            Body = body;
            Version = version;
        }

        public string StreamId { get; set; }

        public object? Body { get; set; }

        public ulong Version { get; set; }
    }
}
