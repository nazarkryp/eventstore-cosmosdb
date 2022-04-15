namespace NKryp.EventStore.Demo.Domain.Events
{
    internal class ItemRemoved : Event
    {
        public string ItemId { get; set; } = null!;
    }
}
