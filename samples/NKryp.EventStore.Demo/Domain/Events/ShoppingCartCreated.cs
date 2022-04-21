namespace NKryp.EventStore.Demo.Domain.Events
{
    internal class ShoppingCartCreated : Event
    {
        public string ShoppingCartId { get; set; } = null!;
    }
}
