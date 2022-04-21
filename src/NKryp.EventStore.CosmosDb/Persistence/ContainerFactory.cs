using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

using NKryp.EventStore.CosmosDb.Options;
using NKryp.EventStore.CosmosDb.Serialization;

namespace NKryp.EventStore.CosmosDb.Persistence
{
    internal interface IContainerFactory
    {
        public Container GetContainer();
    }

    internal class ContainerFactory : IContainerFactory, IDisposable
    {
        private readonly CosmosDbSettings _settings;
        private CosmosClient? _client;
        private bool _disposed;

        public ContainerFactory(IOptions<CosmosDbSettings> options)
        {
            _settings = options.Value;
        }

        private CosmosClient Create() => _client ??= BuildClient(_settings.ConnectionString);

        public Container GetContainer()
        {
            var database = Create().GetDatabase(_settings.DatabaseName);

            return database.GetContainer(_settings.Container);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static CosmosClient BuildClient(string connectionString)
        {
            return new CosmosClient(connectionString, new CosmosClientOptions
            {
                Serializer = new SystemTextJsonCosmosSerializer(new JsonSerializerOptions
                {
                    //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters =
                    {
                        //new TimeSpanConverter(),
                        new JsonStringEnumConverter()
                    }
                }),
#if DEBUG
                // Ignore SSL errors for local CosmosDb Emulator
                HttpClientFactory = () => new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }),
                // Change ConnectionMode for local CosmosDb Emulator
                ConnectionMode = ConnectionMode.Gateway
#endif
            });
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _client?.Dispose();
                    _client = null;
                }

                _disposed = true;
            }
        }
    }
}
