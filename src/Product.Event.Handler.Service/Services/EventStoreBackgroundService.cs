
using EventStore.Client;
using MongoDB.Driver;
using Shared.Events;
using Shared.Services.Abstract;
using System.Reflection;
using System.Text.Json;

namespace Product.Event.Handler.Service.Services
{
    public class EventStoreBackgroundService(IEventStoreService eventStoreService, IMongoDbService mongoDbService) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await eventStoreService.SubscribeToStreamAsync("products-stream", async (streamSubscription, resolvedEvent, cancellationToken) =>
            {
                //hangi event geldiyse ilgili event'e göre işlem yapılacak
                string eventType = resolvedEvent.Event.EventType;

                //shared altındaki bir türle dönüşüm yapıyoruz.
                object @event = JsonSerializer.Deserialize(resolvedEvent.Event.Data.ToArray(), Assembly.Load("Shared").GetTypes().FirstOrDefault(t => t.Name == eventType));


                var productCollection = mongoDbService.GetCollection<Shared.Models.Product>("Products");

                switch (@event)
                {
                    case NewProductAddedEvent e:
                        //MongoDb işlemleri yapılacak
                        //Daha önce oluşturduğumuz product var mı yok mu kontrol ediyoruz

                        var hasProduct = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).AnyAsync();
                        Shared.Models.Product? product = null;

                        if (!hasProduct)
                            await productCollection.InsertOneAsync(new()
                            {
                                Id = e.ProductId,
                                ProductName = e.ProductName,
                                Count = e.InitialCount,
                                IsAvailable = e.IsAvailable,
                                Price = e.InitialPrice
                            });

                        break;

                    //Todo: best practise açıdan aşağıdaki kod tekrarları engellenmeli
                    case CountDecreasedEvent e:
                        product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.Count -= e.DecrementAmount;
                            await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                        }

                        break;
                    case CountIncreasedEvent e:
                        product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.Count += e.IncrementAmount;
                            await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                        }
                        break;
                    case PriceDecreasedEvent e:

                        product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.Price -= e.DecrementAmount;
                            await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                        }
                        break;
                    case PriceIncreasedEvent e:

                        product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.Price += e.IncrementAmount;
                            await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                        }
                        break;
                    case AvailabilityChangedEvent e:

                        product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.IsAvailable = e.IsAvailable;
                            await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                        }
                        break;
                }
            });
        }
    }
}
