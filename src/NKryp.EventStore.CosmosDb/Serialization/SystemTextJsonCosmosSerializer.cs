using System.IO;
using System.Text.Json;

using Microsoft.Azure.Cosmos;

namespace NKryp.EventStore.CosmosDb.Serialization
{
    public class SystemTextJsonCosmosSerializer : CosmosSerializer
    {
        private readonly JsonSerializerOptions? _options;

        public SystemTextJsonCosmosSerializer(JsonSerializerOptions options)
        {
            _options = options;
        }

        public override T FromStream<T>(System.IO.Stream stream)
        {
            using (stream)
            {
                using var memory = new MemoryStream((int)stream.Length);

                stream.CopyTo(memory);

                var utf8Json = memory.ToArray();
                
                var result =  JsonSerializer.Deserialize<T>(utf8Json, _options)!;

                return result;
            }
        }

        public override System.IO.Stream ToStream<T>(T input)
        {
            var utf8Json = JsonSerializer.SerializeToUtf8Bytes(input, _options);

            return new MemoryStream(utf8Json);
        }
    }
}
