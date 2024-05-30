using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Product.Application.Models.ViewModels;
using Shared.Events;
using Shared.Services.Abstract;

namespace Product.Application.Controllers
{
    //Todo Business işlemleri şuan controller üzerinde yapılmaktadır. Bu işlemler bir servis katmanına taşınmalıdır. (best practise)
    public class ProductsController(IEventStoreService eventStoreService, IMongoDbService mongoDbService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var productCollection = mongoDbService.GetCollection<Shared.Models.Product>("Products");
            var products = await (await productCollection.FindAsync(_ => true)).ToListAsync();

            return View(products);
        }

        //Ürün oluşturma sayfası
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVm model)
        {
            NewProductAddedEvent newProductAddedEvent = new()
            {
                ProductId = Guid.NewGuid().ToString(),
                InitialCount = model.Count,
                InitialPrice = model.Price,
                IsAvailable = model.IsAvailable,
                ProductName = model.ProductName
            };

            await eventStoreService.AppendToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(newProductAddedEvent) });

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string productId)
        {
            var productCollection = mongoDbService.GetCollection<Shared.Models.Product>("Products");
            var product = await (await productCollection.FindAsync(p => p.Id == productId)).FirstOrDefaultAsync();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> CountUpdate(Shared.Models.Product model, int status)
        {
            var productCollection = mongoDbService.GetCollection<Shared.Models.Product>("Products");
            var product = await (await productCollection.FindAsync(p => p.Id == model.Id)).FirstOrDefaultAsync();

            if (status == 1)
            {
                CountDecreasedEvent countDecreasedEvent = new()
                {
                    ProductId = model.Id,
                    DecrementAmount = model.Count,
                };
                await eventStoreService.AppendToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(countDecreasedEvent) });
            }
            else if (status == 0)
            {
                CountIncreasedEvent countIncreasedEvent = new()
                {
                    ProductId = model.Id,
                    IncrementAmount = model.Count,
                };

                await eventStoreService.AppendToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(countIncreasedEvent) });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PriceUpdate(Shared.Models.Product model, int status)
        {
            var productCollection = mongoDbService.GetCollection<Shared.Models.Product>("Products");
            var product = await (await productCollection.FindAsync(p => p.Id == model.Id)).FirstOrDefaultAsync();

            if (status==1)
            {
                PriceDecreasedEvent priceDecreasedEvent = new()
                {
                    ProductId = model.Id,
                    DecrementAmount = model.Price,
                };
                await eventStoreService.AppendToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(priceDecreasedEvent) });
            }

            else if (status == 0)
            {
                PriceIncreasedEvent priceIncreasedEvent = new()
                {
                    ProductId = model.Id,
                    IncrementAmount = model.Price,
                };

                await eventStoreService.AppendToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(priceIncreasedEvent) });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AvailableUpdate(Shared.Models.Product model)
        {
            var productCollection = mongoDbService.GetCollection<Shared.Models.Product>("Products");
            var product = await (await productCollection.FindAsync(p => p.Id == model.Id)).FirstOrDefaultAsync();

            if (product.IsAvailable != model.IsAvailable)
            {
                AvailabilityChangedEvent availabilityChangedEvent = new()
                {
                    ProductId = model.Id,
                    IsAvailable = model.IsAvailable
                };

                await eventStoreService.AppendToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(availabilityChangedEvent) });
            }

            return RedirectToAction("Index");
        }
    }
}
