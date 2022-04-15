namespace NKryp.EventStore.CosmosDb.Abstractions
{
    public interface IEventSerializer
    {
        T Deserialize<T>(EventData eventData) where T : class;
    }
}
