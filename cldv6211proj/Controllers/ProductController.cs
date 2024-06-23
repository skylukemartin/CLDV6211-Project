using System.Diagnostics;
using cldv6211proj.Models;
using cldv6211proj.Models.Database;
using cldv6211proj.Models.ViewModels;
using cldv6211proj.Services;
using Microsoft.AspNetCore.Mvc;

namespace cldv6211proj.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult MyWork()
        {
            return View(new ProductList() { Products = _productService.GetProducts() });
        }

        [HttpGet]
        public IActionResult CraftProduct()
        {
            return View(new Product());
        }

        [HttpPost]
        public IActionResult FinishCraftProduct(Product product)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("ContactUs", "Home");
            var userID = HttpContext.Session.GetInt32("userID") ?? -1;
            if (userID < 1)
                return RedirectToAction("Login", "Home");
            product.UserID = userID;
            if (!_productService.AddProduct(product))
                return RedirectToAction("ContactUs", "Home");
            return RedirectToAction("OrderHistory", "Order");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                }
            );
        }
    }
}
