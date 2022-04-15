namespace NKryp.EventStore.CosmosDb.Exceptions
{
    [Serializable]
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string streamId, ulong expectedVersion, object version)
        {
            StreamId = streamId;
            ExpectedVersion = expectedVersion;
            Version = version;
        }

        public string StreamId { get; }

        public ulong ExpectedVersion { get; }

        public object Version { get; }
    }
}
