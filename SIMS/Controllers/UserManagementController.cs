using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIMS.Models;
using SIMS.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SIMS.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IDepartmentService _departmentService;

        public UserManagementController(
            IUserManagementService userManagementService,
            IDepartmentService departmentService)
        {
            _userManagementService = userManagementService;
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            var users = await _userManagementService.GetAllAsync();
            var userViewModels = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                UserCode = u.UserCode,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role
            }).ToList();
            return View(userViewModels);
        }

        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            ViewBag.Roles = new SelectList(new[] { "Admin", "Instructor", "Student" });
            ViewBag.Departments = new SelectList(await _departmentService.GetAllAsync(), "DepartmentId", "DepartmentName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserCode = model.UserCode,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = model.Password,
                    DepartmentId = model.DepartmentId,
                    Role = model.Role
                };
                var result = await _userManagementService.AddUserAsync(user);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = "User created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.ErrorMessage);
            }

            ViewBag.Roles = new SelectList(new[] { "Admin", "Instructor", "Student" }, model.Role);
            ViewBag.Departments = new SelectList(await _departmentService.GetAllAsync(), "DepartmentId", "DepartmentName", model.DepartmentId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            var user = await _userManagementService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserCode = user.UserCode,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DepartmentId = user.DepartmentId,
                Role = user.Role
            };

            ViewBag.Roles = new SelectList(new[] { "Admin", "Instructor", "Student" }, model.Role);
            ViewBag.Departments = new SelectList(await _departmentService.GetAllAsync(), "DepartmentId", "DepartmentName", model.DepartmentId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditViewModel model)
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Id = model.Id,
                    Email = model.Email,
                    UserCode = model.UserCode,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DepartmentId = model.DepartmentId,
                    Role = model.Role
                };
                var result = await _userManagementService.UpdateUserAsync(user);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = "User updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.ErrorMessage);
            }

            ViewBag.Roles = new SelectList(new[] { "Admin", "Instructor", "Student" }, model.Role);
            ViewBag.Departments = new SelectList(await _departmentService.GetAllAsync(), "DepartmentId", "DepartmentName", model.DepartmentId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            var user = await _userManagementService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            var (success, error) = await _userManagementService.DeleteUserAsync(id);
            if (success)
                TempData["SuccessMessage"] = "User deleted successfully!";
            else
                TempData["ErrorMessage"] = error ?? "Failed to delete user.";

            return RedirectToAction(nameof(Index));
        }

        public class UserCreateViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            public string UserCode { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Required]
            public string Role { get; set; }
            public int? DepartmentId { get; set; }
        }

        public class UserEditViewModel
        {
            public int Id { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            public string UserCode { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string Role { get; set; }
            public int? DepartmentId { get; set; }
        }

        public class UserViewModel
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string UserCode { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Role { get; set; }
        }
    }
}