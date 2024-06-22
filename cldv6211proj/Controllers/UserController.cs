using System.Diagnostics;
using cldv6211proj.Models;
using Microsoft.AspNetCore.Mvc;

namespace cldv6211proj.Controllers
{
    using cldv6211proj.Services;

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
            return View(new LoginUserModel());
        }

        [HttpPost]
        public IActionResult Login(LoginUserModel user)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("ContactUs", "Home");

            int userID = _userService.LoginUser(user.Email!, user.Password!); // [Required]!
            if (userID < 0)
                return RedirectToAction("ContactUs", "Home");

            HttpContext.Session.SetInt32("userID", userID);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterUserModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterUserModel user)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("ContactUs", "Home");

            int userID = _userService.CreateUser(
                user.Name!, // ? [Required]!
                user.Surname!, // ? [Required]!
                user.Email!, // ? [Required]!
                user.Password! // ? [Required]!
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
