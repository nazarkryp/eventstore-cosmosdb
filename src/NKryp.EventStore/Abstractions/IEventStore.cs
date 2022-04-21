using System.Threading.Tasks;

namespace NKryp.EventStore.Abstractions
{
    public interface IEventStore
    {
        Task<EventStream?> ReadStreamAsync(string streamId);

        Task AppendStreamAsync(string streamId, EventData[] events, ulong? version);

        Task<StreamHeader?> ReadStreamHeaderAsync(string streamId);
    }
}
