namespace NKryp.EventStore.Demo.Domain.Events
{
    internal class QuantityChanged : Event
    {
        public string ItemId { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
