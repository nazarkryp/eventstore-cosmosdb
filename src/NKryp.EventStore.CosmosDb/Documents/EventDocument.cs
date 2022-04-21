using System;
using System.Text.Json.Serialization;

namespace NKryp.EventStore.CosmosDb.Documents
{
    internal class EventDocument
    {
        private const char Separator = '_';

        public EventDocument(DocumentType documentType)
            => DocumentType = documentType;

        [JsonPropertyName("id")]
        public virtual string Id => GenerateId();

        public DocumentType DocumentType { get; }

        public string StreamId { get; set; } = null!;

        public ulong Version { get; set; }

        public string BodyType { get; set; } = null!;

        public object Body { get; set; } = null!;

        public decimal SortOrder => Version + GetOrderingFraction(DocumentType);

        [JsonPropertyName("_etag")]
        public string? ETag { get; set; }

        [JsonPropertyName("_ts")]
        public ulong Timestamp { get; set; }

        internal string GenerateId()
        {
            return DocumentType switch
            {
                DocumentType.Header => StreamId,
                DocumentType.Event => GenerateEventId(StreamId, Version),
                DocumentType.Snapshot => $"{StreamId}{Separator}{Version}{Separator}S",
                _ => throw new NotSupportedException($"Document type '{DocumentType}' is not supported.")
            };
        }

        internal static string GenerateEventId(string streamId, ulong version) => $"{streamId}{Separator}{version}";

        internal static decimal GetOrderingFraction(DocumentType documentType)
        {
            switch (documentType)
            {
                case DocumentType.Header:
                    return 0.3M;
                case DocumentType.Snapshot:
                    return 0.2M;
                case DocumentType.Event:
                    return 0.1M;
                default:
                    throw new NotSupportedException($"Document type '{documentType}' is not supported.");
            }
        }
    }
}
