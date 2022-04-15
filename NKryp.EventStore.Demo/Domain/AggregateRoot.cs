namespace NKryp.EventStore.Demo.Domain
{
    public abstract class AggregateRoot
    {
        private readonly List<Event> _events = new();

        public Event[] GetEvents()
            => _events.ToArray();

        protected void AddEvent(Event @event)
            => _events.Add(@event);

        //protected void Apply(Event @event)
        //    => Mutate(@event);

        protected void Apply(Event @event)
        {
            Mutate(@event);
            _events.Add(@event);
        }

        protected abstract void Mutate(Event @event);
    }
}
