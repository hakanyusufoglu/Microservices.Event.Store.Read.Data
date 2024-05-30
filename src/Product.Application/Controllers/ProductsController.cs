using Microsoft.AspNetCore.Mvc;
using Product.Application.Models.ViewModels;
using Shared.Events;
using Shared.Services.Abstract;

namespace Product.Application.Controllers
{
	public class ProductsController(IEventStoreService eventStoreService) : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		//Ürün oluşturma sayfası
		public IActionResult CreateProduct()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreateProduct(CreateProductVm model)
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

			return RedirectToAction("CreateProduct");
		}
	}
}
