using Microsoft.AspNetCore.Mvc;
using SIMS.Models;
using SIMS.Services.Interfaces;

namespace SIMS.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUserManagementService _userService;

        public DashboardController(IUserManagementService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userService.GetCurrentUserAsync(HttpContext);
            if (user == null)
                return RedirectToAction("Login", "Account");

            switch (user.Role)
            {
                case "Admin":
                    return RedirectToAction("Admin");
                case "Instructor":
                    return RedirectToAction("Instructor");
                case "Student":
                    return RedirectToAction("Student");
                default:
                    return RedirectToAction("Login", "Account");
            }
        }

        public async Task<IActionResult> Admin()
        {
            var user = await _userService.GetCurrentUserAsync(HttpContext);
            if (user == null || user.Role != "Admin")
                return RedirectToAction("Login", "Account");

            return View(user);
        }

        public async Task<IActionResult> Instructor()
        {
            var user = await _userService.GetCurrentUserAsync(HttpContext);
            if (user == null || user.Role != "Instructor")
                return RedirectToAction("Login", "Account");

            return View(user);
        }

        public async Task<IActionResult> Student()
        {
            var user = await _userService.GetCurrentUserAsync(HttpContext);
            if (user == null || user.Role != "Student")
                return RedirectToAction("Login", "Account");

            return View(user);
        }
    }
}