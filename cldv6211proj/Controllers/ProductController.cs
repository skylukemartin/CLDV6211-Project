using System.Diagnostics;
using cldv6211proj.Models;
using cldv6211proj.Models.Db;
using Microsoft.AspNetCore.Mvc;

namespace cldv6211proj.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        public IActionResult MyWork()
        {
            ViewData["Products"] = ProductManager.GetAllProducts();
            ViewData["User"] = UserManager.FindUser(HttpContext.Session.GetInt32("userID") ?? -1);
            return View();
        }

        [HttpGet]
        public IActionResult CraftProduct()
        {
            return View(new Product());
        }

        [HttpPost]
        public IActionResult FinishCraftProduct(Product product)
        {
            var userID = HttpContext.Session.GetInt32("userID") ?? -1;
            if (userID < 1)
                return RedirectToAction("Login", "Home");
            product.UserID = userID;
            if (!ProductManager.AddProduct(product))
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
