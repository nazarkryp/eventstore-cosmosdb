namespace NKryp.EventStore.CosmosDb.Abstractions
{
    public interface ITypeProvider
    {
        string GetEventId(Type type);

        Type GetType(string id);
    }
}
