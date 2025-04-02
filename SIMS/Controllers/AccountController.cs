using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIMS.Models;
using SIMS.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SIMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserManagementService _userService;

        public AccountController(IUserManagementService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _userService.LoginAsync(model.Email, model.Password))
            {
                var user = await _userService.GetByEmailAsync(model.Email);
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Role", user.Role);
                return RedirectToAction("Index", "Dashboard"); // Chuyển đến Dashboard
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}