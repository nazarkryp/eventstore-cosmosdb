namespace NKryp.EventStore.CosmosDb.Options
{
    public class CosmosDbSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string Container { get; set; } = null!;
    }
}
