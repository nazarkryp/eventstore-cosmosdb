namespace NKryp.EventStore.CosmosDb.Options
{
    public class EventStoreOptions
    {
        public byte BatchSize { get; set; } = 100;

        public int QueryMaxItemCount { get; set; } = 1000;
    }
}
