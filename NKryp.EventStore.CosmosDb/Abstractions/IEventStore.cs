namespace NKryp.EventStore.CosmosDb.Abstractions
{
    public interface IEventStore
    {
        Task LoadStreamHeaders(string query, Func<IReadOnlyCollection<StreamHeader>, Task> callback);

        Task<StreamHeader?> GetStreamHeaderAsync(string streamId);

        Task<EventStream?> ReadStreamAsync(string streamId);

        Task AppendStreamAsync(string streamId, EventData[] events, ulong? expectedVersion = null);
    }
}
