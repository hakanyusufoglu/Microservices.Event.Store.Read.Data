using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Product.Application.Models.ViewModels;
using Shared.Events;
using Shared.Services.Abstract;

namespace Product.Application.Controllers
{
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
	}
}
