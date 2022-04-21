using System;

namespace NKryp.EventStore.CosmosDb.Exceptions
{
    [Serializable]
    internal class TypeNotFoundException : Exception
    {
        public TypeNotFoundException(string typeName)
        {
            TypeName = typeName;
        }

        public string TypeName { get; }
    }
}
