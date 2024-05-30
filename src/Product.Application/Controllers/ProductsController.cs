using Microsoft.AspNetCore.Mvc;
using Product.Application.Models.ViewModels;

namespace Product.Application.Controllers
{
	public class ProductsController : Controller
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
		public IActionResult CreateProduct(CreateProductVm model)
		{
			return View();
		}
	}
}
