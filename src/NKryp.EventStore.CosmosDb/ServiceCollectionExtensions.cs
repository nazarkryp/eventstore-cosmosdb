using System;

using Microsoft.Extensions.DependencyInjection;

using NKryp.EventStore.Abstractions;
using NKryp.EventStore.CosmosDb.Options;
using NKryp.EventStore.CosmosDb.Persistence;
using NKryp.EventStore.CosmosDb.Providers;
using NKryp.EventStore.CosmosDb.Serialization;

namespace NKryp.EventStore.CosmosDb
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosDbEventStore(this IServiceCollection services, Action<CosmosDbSettings> configureSettings)
        {
            services.Configure(configureSettings);

            services.AddSingleton<IDocumentSerializer, EventDocumentSerializer>();
            services.AddTransient<EventDocumentSerializer>();
            services.AddSingleton<ITypeProvider, PlatformTypeProvider>();
            services.AddSingleton<IContainerFactory, ContainerFactory>();
            services.AddTransient<IEventStore, EventStore>();

            return services;
        }
    }
}
