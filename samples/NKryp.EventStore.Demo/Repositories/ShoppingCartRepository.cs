using NKryp.EventStore.Abstractions;
using NKryp.EventStore.Demo.Domain;

namespace NKryp.EventStore.Demo.Repositories
{
    internal class ShoppingCartRepository
    {
        private readonly IEventStore _eventStore;

        public ShoppingCartRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<ShoppingCartAggregateRoot?> FindAsync(string shoppingCartId)
        {
            var stream = await _eventStore.ReadStreamAsync(shoppingCartId);

            if (stream == null)
            {
                return null;
            }

            return ShoppingCartAggregateRoot.Aggregate(stream.Events.Select(e => (Event)e.Body!).ToArray());
        }

        public async Task SaveAsync(ShoppingCartAggregateRoot aggregateRoot)
        {
            var header = await _eventStore.ReadStreamHeaderAsync(aggregateRoot.Id);
            var version = (header?.Version).GetValueOrDefault() + 1;

            var events = aggregateRoot.GetUncommitedEvents()
                .Select(@event => new EventData(aggregateRoot.Id, @event, version++))
                .ToArray();

            await _eventStore.AppendStreamAsync(aggregateRoot.Id, events, header?.Version);

            aggregateRoot.Commit();
        }
    }
}
