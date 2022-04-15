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

                //using var client = new CosmosClient(connectionString);

                //var databaseResponse = client
                //    .CreateDatabaseIfNotExistsAsync(databaseName)
                //    .GetAwaiter()
                //    .GetResult();

                //try
                //{
                //    var container = databaseResponse.Database.GetContainer(containerName);
                //    container.DeleteContainerAsync();
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e);
                //    throw;
                //}

                //databaseResponse.Database
                //    .CreateContainerIfNotExistsAsync(new ContainerProperties(containerName, "/StreamId"))
                //    .GetAwaiter()
                //    .GetResult();

                services.AddCosmosDbEventStore(options =>
                {
                    options.ConnectionString = connectionString;
                    options.DatabaseName = databaseName;
                    options.Container = containerName;
                });
            });
    }
}