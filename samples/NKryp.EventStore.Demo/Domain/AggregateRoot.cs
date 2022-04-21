namespace NKryp.EventStore.Demo.Domain
{
    public abstract class AggregateRoot
    {
        private readonly List<Event> _events = new();

        public Event[] GetUncommitedEvents()
            => _events.ToArray();

        public void Commit()
        {
            _events.Clear();
        }

        protected void Apply(Event @event)
        {
            Mutate(@event);
            _events.Add(@event);
        }

        protected abstract void Mutate(Event @event);
    }
}
