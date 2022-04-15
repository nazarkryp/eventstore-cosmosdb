using Newtonsoft.Json;

using NKryp.EventStore.CosmosDb.Abstractions;
using NKryp.EventStore.CosmosDb.Providers;

namespace NKryp.EventStore.CosmosDb.Options
{
    public class EventStoreOptions
    {
        public byte BatchSize { get; set; } = 100;

        public int QueryMaxItemCount { get; set; } = 1000;

        public JsonSerializer JsonSerializer { get; set; } = JsonSerializer.CreateDefault();

        public ITypeProvider TypeProvider { get; set; } = new PlatformTypeProvider();

        public bool IgnoreMissingTypes { get; set; } = false;
    }
}
