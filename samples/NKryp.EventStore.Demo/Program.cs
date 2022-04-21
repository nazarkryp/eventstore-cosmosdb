using System.Net;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NKryp.EventStore.CosmosDb;
using NKryp.EventStore.Demo.Repositories;

namespace NKryp.EventStore.Demo;

public static class Program
{
    private static async Task Main()
    {
        var host = CreateHostBuilder()
            .Build();

        try
        {
            await host.Services.GetRequiredService<DemoService>().RunAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine();
            Console.WriteLine(e.StackTrace);
        }
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder =>
            {
                builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services
                    .AddTransient<DemoService>()
                    .AddTransient<ShoppingCartRepository>();

                var cosmosDbSection = configuration.GetSection("CosmosDb");
                var connectionString = cosmosDbSection.GetValue<string>("ConnectionString");
                var databaseName = cosmosDbSection.GetValue<string>("DatabaseName");
                var containerName = cosmosDbSection.GetValue<string>("Container");

                // CreatePersistence(connectionString, databaseName, containerName);

                services.AddCosmosDbEventStore(options =>
                {
                    options.ConnectionString = connectionString;
                    options.DatabaseName = databaseName;
                    options.Container = containerName;
                });
            });
    }

    private static void CreatePersistence(string connectionString, string databaseName, string containerName)
    {
        using var client = new CosmosClient(connectionString);

        Console.WriteLine($"Database '{databaseName}' initiating");

        var database = client
            .CreateDatabaseIfNotExistsAsync(databaseName)
            .GetAwaiter()
            .GetResult();

        Console.WriteLine($"Database '{databaseName}' initiated");

        try
        {
            Console.WriteLine("Deleting old container");

            var container = database.Database.GetContainer(containerName);
            container.DeleteContainerAsync().GetAwaiter().GetResult();
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine("Container not found");
        }

        Console.WriteLine($"Creating new container: '{containerName}'");

        database.Database
            .CreateContainerIfNotExistsAsync(new ContainerProperties(containerName, "/StreamId"))
            .GetAwaiter()
            .GetResult();

        Console.WriteLine($"Container '{containerName}' created");
    }
}