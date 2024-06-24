using System.Diagnostics;
using cldv6211proj.Models;
using cldv6211proj.Models.Database;
using cldv6211proj.Models.ViewModels;
using cldv6211proj.Services;
using Microsoft.AspNetCore.Mvc;

namespace cldv6211proj.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public OrderController(
            IOrderService orderService,
            IUserService userService,
            IProductService productService,
            ILogger<ProductController> logger
        )
        {
            _orderService = orderService;
            _userService = userService;
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult PlaceOrder(int productID)
        {
            var user = _userService.GetUser(HttpContext.Session.GetInt32("userID") ?? -1);
            if (user == null)
                return RedirectToAction("Register", "User");

            var product = _productService.GetProduct(productID);
            if (product == null)
                return RedirectToAction("ContactUs", "Home");

            return View(new OrderPlace { User = user, Product = product });
        }

        [HttpPost]
        public IActionResult SubmitOrder(OrderSubmitForm orderForm)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("ContactUs", "Home");
            // TODO: is there a [Required] type data annotation thing for a minimum value?
            if (orderForm.Quantity < 1)
                return RedirectToAction("ContactUs", "Home");

            var orderID = _orderService.CreateOrder(orderForm);
            if (orderID < 0)
                return RedirectToAction("ContactUs", "Home");

            return RedirectToAction("OrderHistory", "Order");
        }

        [HttpGet]
        public IActionResult OrderHistory()
        {
            var user = _userService.GetUser(HttpContext.Session.GetInt32("userID") ?? -1);
            if (user == null)
                return RedirectToAction("ContactUs", "Home");
            var orderInfos = _orderService.FindOrderInfos(user.ID, isBuyer: true);
            if (orderInfos == null)
                return RedirectToAction("ContactUs", "Home");
            return View(new UserOrderInfos() { User = user, OrderInfos = orderInfos });
        }

        [HttpGet]
        public IActionResult ProcessOrders()
        {
            var user = _userService.GetUser(HttpContext.Session.GetInt32("userID") ?? -1);
            if (user == null)
                return RedirectToAction("ContactUs", "Home");
            var orderInfos = _orderService.FindOrderInfos(user.ID, isBuyer: false);
            if (orderInfos == null)
                return RedirectToAction("ContactUs", "Home");
            return View(new UserOrderInfos() { User = user, OrderInfos = orderInfos });
        }

        [HttpPost]
        public IActionResult ShipOrder(int orderID)
        {
            _orderService.ProcessOrder(orderID);
            return RedirectToAction("ProcessOrders", "Order");
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
