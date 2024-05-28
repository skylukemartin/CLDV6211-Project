using System.Diagnostics;
using cldv6211proj.Models;
using cldv6211proj.Models.Db;
using Microsoft.AspNetCore.Mvc;

namespace cldv6211proj.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<ProductController> _logger;

        public OrderController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult PlaceOrder(int productID)
        {
            var user = UserManager.FindUser(HttpContext.Session.GetInt32("userID") ?? -1);
            if (user == null)
                return RedirectToAction("SignUp", "User");
            var product = ProductManager.FindProduct(productID);
            if (product == null)
                return RedirectToAction("ContactUs", "Home");

            return View(
                new SubmitOrderModel
                {
                    User = user,
                    Product = product,
                    OrderForm = new()
                }
            );
        }

        [HttpPost]
        public IActionResult SubmitOrder(int productID, SubmitOrderForm orderForm)
        {
            if (orderForm.Quantity < 1)
                return RedirectToAction("ContactUs", "Home");
            var orderID = OrderManager.CreateOrder(
                HttpContext.Session.GetInt32("userID") ?? -1,
                productID,
                orderForm.Address,
                orderForm.Quantity
            );
            if (orderID < 1)
                return RedirectToAction("ContactUs", "Home");
            return RedirectToAction("OrderHistory", "Order");
        }

        [HttpGet]
        public IActionResult OrderHistory()
        {
            var user = UserManager.FindUser(HttpContext.Session.GetInt32("userID") ?? -1);
            if (user == null)
                return RedirectToAction("ContactUs", "Home");
            var orderInfos = OrderManager.ClientOrderInfos(user.ID);
            if (orderInfos == null)
                return RedirectToAction("ContactUs", "Home");
            return View(new UserOrdersModel() { User = user, OrderInfos = orderInfos });
        }

        [HttpGet]
        public IActionResult ProcessOrders()
        {
            var user = UserManager.FindUser(HttpContext.Session.GetInt32("userID") ?? -1);
            if (user == null)
                return RedirectToAction("ContactUs", "Home");
            var orderInfos = OrderManager.SellerOrderInfos(user.ID);
            if (orderInfos == null)
                return RedirectToAction("ContactUs", "Home");
            return View(new UserOrdersModel() { User = user, OrderInfos = orderInfos });
        }

        [HttpPost]
        public IActionResult ShipOrder(int orderID)
        {
            OrderManager.ProcessOrder(orderID);
            return RedirectToAction("ContactUs", "Home");
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
