namespace NKryp.EventStore.Demo.Domain.Events
{
    internal class ItemAdded : Event
    {
        public string ItemId { get; set; } = null!;
    }
}
