using System;
using System.Collections.Concurrent;

using NKryp.EventStore.Abstractions;

namespace NKryp.EventStore.CosmosDb.Providers
{
    internal class PlatformTypeProvider : ITypeProvider
    {
        private readonly ConcurrentDictionary<string, Type> _cache = new ConcurrentDictionary<string, Type>();

        public string GetEventTypeId(Type type) => type.AssemblyQualifiedName!;

        public Type GetType(string eventTypeId) => _cache.GetOrAdd(eventTypeId, t => Type.GetType(t)!);
    }
}
