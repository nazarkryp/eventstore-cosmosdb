using System.Text.Json;

using NKryp.EventStore.Demo.Domain;
using NKryp.EventStore.Demo.Repositories;

namespace NKryp.EventStore.Demo
{
    internal class DemoService
    {
        private readonly ShoppingCartRepository _shoppingCartRepository;

        public DemoService(ShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

        public async Task RunAsync()
        {
            var aggregateRoot = ShoppingCartAggregateRoot.Create();

            aggregateRoot.AddItem("p1");
            aggregateRoot.AddItem("p2");
            aggregateRoot.UpdateItemQuantity("p1", 5);
            aggregateRoot.AddItem("p3");
            aggregateRoot.RemoveItem("p2");
            aggregateRoot.AddItem("p4");
            aggregateRoot.RemoveItem("p1");
            aggregateRoot.UpdateItemQuantity("p4", 10);

            await _shoppingCartRepository.SaveAsync(aggregateRoot);

            aggregateRoot.AddItem("p10");
            aggregateRoot.UpdateItemQuantity("p10", 100);

            await _shoppingCartRepository.SaveAsync(aggregateRoot);

            aggregateRoot.AddItem("p100500");
            aggregateRoot.UpdateItemQuantity("p100500", 100500);

            await _shoppingCartRepository.SaveAsync(aggregateRoot);

            aggregateRoot = await _shoppingCartRepository.FindAsync(aggregateRoot.Id);

            Console.WriteLine(JsonSerializer.Serialize(aggregateRoot, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }
    }
}
