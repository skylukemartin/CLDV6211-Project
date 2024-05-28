using System.Diagnostics;
using cldv6211proj.Models;
using Microsoft.AspNetCore.Mvc;

namespace cldv6211proj.Controllers
{
    using Models.Db;

    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new User());
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            User? userLogin = UserManager.Login(user);
            if (userLogin == null)
                return RedirectToAction("ContactUs", "Home");
            HttpContext.Session.SetInt32("userID", userLogin.ID);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new User());
        }

        [HttpPost]
        public IActionResult SignUp(User user)
        {
            User? userSignup = UserManager.Signup(user);
            if (userSignup == null)
                return RedirectToAction("ContactUs", "Home");
            HttpContext.Session.SetInt32("userID", userSignup.ID);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
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
