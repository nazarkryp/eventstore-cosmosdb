namespace NKryp.EventStore.Demo.Domain
{
    public abstract class Event
    {
        public bool Processed { get; set; }
    }
}
