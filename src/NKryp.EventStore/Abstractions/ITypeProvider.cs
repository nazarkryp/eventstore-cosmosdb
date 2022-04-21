using System;

namespace NKryp.EventStore.Abstractions
{
    public interface ITypeProvider
    {
        string GetEventTypeId(Type type);

        Type GetType(string eventTypeId);
    }
}
