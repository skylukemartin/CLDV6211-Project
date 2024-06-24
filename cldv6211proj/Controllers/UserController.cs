using System.Diagnostics;
using cldv6211proj.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace cldv6211proj.Controllers
{
    using cldv6211proj.Models;
    using cldv6211proj.Services;
    using Shared.Services;

    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new UserLogin());
        }

        [HttpPost]
        public IActionResult Login(UserLogin userLogin)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("ContactUs", "Home");

            int userID = _userService.LoginUser(userLogin.Email!, userLogin.Password!);
            if (userID < 0)
                return RedirectToAction("ContactUs", "Home");

            HttpContext.Session.SetInt32("userID", userID);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new UserRegister());
        }

        [HttpPost]
        public IActionResult Register(UserRegister userRegister)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("ContactUs", "Home");

            int userID = _userService.CreateUser(
                userRegister.Name!,
                userRegister.Surname!,
                userRegister.Email!,
                userRegister.Password!
            );
            if (userID < 0)
                return RedirectToAction("ContactUs", "Home");

            HttpContext.Session.SetInt32("userID", userID);
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
