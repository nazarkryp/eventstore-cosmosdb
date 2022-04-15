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
            var shoppingCart = new ShoppingCartAggregateRoot
            {
                Id = Guid.NewGuid().ToString()
            };

            shoppingCart.AddEvent(new ShoppingCartCreated
            {
                ShoppingCartId = shoppingCart.Id
            });

            return shoppingCart;
        }

        public void AddItem(string itemId)
        {
            _items.Add(new ShoppingCartItem
            {
                ItemId = itemId,
                Quantity = 1
            });

            AddEvent(new ItemAdded
            {
                ItemId = itemId
            });
        }

        public void UpdateItemQuantity(string itemId, int quantity)
        {
            _items.First(e => e.ItemId == itemId).Quantity = quantity;

            AddEvent(new QuantityChanged
            {
                ItemId = itemId,
                Quantity = quantity
            });
        }

        public void RemoveItem(string itemId)
        {
            var item = _items.First(e => e.ItemId == itemId);
            _items.Remove(item);

            AddEvent(new ItemRemoved
            {
                ItemId = itemId
            });
        }

        public static ShoppingCartAggregateRoot Aggregate(Event[] events)
        {
            var shoppingCart = new ShoppingCartAggregateRoot();

            foreach (var @event in events)
            {
                shoppingCart.Mutate(@event!);
            }

            return shoppingCart;
        }

        protected override void Mutate(Event @event)
        {
            if (@event is ShoppingCartCreated created)
            {
                Id = created.ShoppingCartId;
            }
            else if (@event is ItemAdded added)
            {
                _items.Add(new ShoppingCartItem
                {
                    ItemId = added.ItemId,
                    Quantity = 1
                });
            }
            else if (@event is ItemRemoved removed)
            {
                var item = _items.First(e => e.ItemId == removed.ItemId);
                _items.Remove(item);
            }
            else if (@event is QuantityChanged quantityChanged)
            {
                var item = _items.First(e => e.ItemId == quantityChanged.ItemId);
                item.Quantity = quantityChanged.Quantity;
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
