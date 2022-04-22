# NKryp.EventStore.CosmosDb

.Net Library that allows easy implement event sourcing using CosmosDb

## Installation

- [NKryp.EventStore.CosmosDb](https://www.nuget.org/packages/NKryp.EventStore.CosmosDb/) ![Nuget](https://img.shields.io/nuget/v/NKryp.EventStore.CosmosDb?style=plastic)



> Install-Package NKryp.EventStore.CosmosDb

## 

## Usage

Configuring dependency injection

```dotnet
services.AddCosmosDbEventStore(options =>
{
    options.ConnectionString = connectionString;
    options.DatabaseName = databaseName;
    options.Container = containerName;
});
```

Then some where in your  code

```dotnet
public class ShoppingCartRepository
{
	private readonly IEventStore _eventStore;

	public ShoppingCartRepository(IEventStore eventStore)
	{
		_eventStore = eventStore;
	}
	
	public async Task<ShoppingCartAggregateRoot?> FindAsync(string shoppingCartId)
	{
		var stream = await _eventStore.ReadStreamAsync(shoppingCartId);
		
		if (stream == null)
		{
			return null;
		}
		
		return ShoppingCartAggregateRoot.Aggregate(stream.Events.Select(e => (Event)e.Body!).ToArray());
	}

	public async Task SaveAsync(ShoppingCartAggregateRoot aggregateRoot)
	{
		var header = await _eventStore.ReadStreamHeaderAsync(aggregateRoot.Id);
		var version = (header?.Version).GetValueOrDefault() + 1;

		var events = aggregateRoot.GetUncommitedEvents()
			.Select(@event => new EventData(aggregateRoot.Id, @event, version++))
			.ToArray();

		await _eventStore.AppendStreamAsync(aggregateRoot.Id, events, header?.Version);
	}
}
```


