namespace NKryp.EventStore.CosmosDb.Options
{
    public class ReadStreamOptions
    {
        public ulong? FromVersion { get; set; } = null;

        public ulong? ToVersion { get; set; } = null;

        public int? MaxItemCount { get; set; } = null;
    }
}
