using System.Collections.Concurrent;

using NKryp.EventStore.CosmosDb.Abstractions;

namespace NKryp.EventStore.CosmosDb.Providers
{
    public class PlatformTypeProvider : ITypeProvider
    {
        private readonly ConcurrentDictionary<string, Type> _cache = new();

        public string GetEventId(Type type) => type.AssemblyQualifiedName!;

        public Type GetType(string identifier) => _cache.GetOrAdd(identifier, t => Type.GetType(t)!);
    }
}
