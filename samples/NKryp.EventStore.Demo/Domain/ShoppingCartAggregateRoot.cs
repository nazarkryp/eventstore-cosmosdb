using NKryp.EventStore.Demo.Domain.Events;

namespace NKryp.EventStore.Demo.Domain
{
    public class ShoppingCartAggregateRoot : AggregateRoot
    {
        private readonly List<ShoppingCartItem> _items = new();

        private ShoppingCartAggregateRoot()
        {
        }

        public string Id { get; set; } = null!;

        public IReadOnlyCollection<ShoppingCartItem> Items => _items.AsReadOnly();

        public static ShoppingCartAggregateRoot Create()
        {
            var shoppingCart = new ShoppingCartAggregateRoot();

            shoppingCart.Apply(new ShoppingCartCreated
            {
                ShoppingCartId = Guid.NewGuid().ToString()
            });

            return shoppingCart;
        }

        public void AddItem(string itemId)
            => Apply(new ItemAdded
            {
                ItemId = itemId
            });

        public void UpdateItemQuantity(string itemId, int quantity)
            => Apply(new QuantityChanged
            {
                ItemId = itemId,
                Quantity = quantity
            });

        public void RemoveItem(string itemId)
            => Apply(new ItemRemoved
            {
                ItemId = itemId
            });

        public static ShoppingCartAggregateRoot Aggregate(Event[] events)
        {
            var shoppingCart = new ShoppingCartAggregateRoot();

            foreach (var @event in events)
            {
                shoppingCart.Mutate(@event);
            }

            return shoppingCart;
        }

        protected override void Mutate(Event @event)
        {
            switch (@event)
            {
                case ShoppingCartCreated created:
                    When(created);
                    break;
                case ItemAdded added:
                    When(added);
                    break;
                case ItemRemoved removed:
                    When(removed);
                    break;
                case QuantityChanged quantityChanged:
                    When(quantityChanged);
                    break;
            }
        }

        private void When(ShoppingCartCreated created)
            => Id = created.ShoppingCartId;

        private void When(ItemAdded added)
            => _items.Add(new ShoppingCartItem
            {
                ItemId = added.ItemId,
                Quantity = 1
            });

        private void When(QuantityChanged quantityChanged)
            => _items.First(e => e.ItemId == quantityChanged.ItemId).Quantity = quantityChanged.Quantity;

        private void When(ItemRemoved removed)
            => _items.Remove(_items.First(e => e.ItemId == removed.ItemId));
    }
}
