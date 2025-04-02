using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIMS.Models;
using SIMS.Repositories.Interfaces;
using SIMS.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SIMS.Controllers
{
    public class CourseManagementController : Controller
    {
        private readonly ICourseManagementService _courseManagementService;
        private readonly IDepartmentService _departmentService;
        private readonly IUserManagementService _userManagementService;

        public CourseManagementController(
            ICourseManagementService courseManagementService,
            IDepartmentService departmentService,
            IUserManagementService userManagementService)
        {
            _courseManagementService = courseManagementService;
            _departmentService = departmentService;
            _userManagementService = userManagementService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (user == null || user.Role != "Admin")
                return RedirectToAction("Login", "Account");

            var courses = await _courseManagementService.GetCourseWithDepartmentAsync();
            var courseViewModels = courses.Select(c => new CourseViewModel
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                Credits = c.Credits,
                DepartmentName = c.Department?.DepartmentName ?? "N/A"
            }).ToList();

            return View(courseViewModels);
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (user == null || user.Role != "Admin")
                return RedirectToAction("Login", "Account");

            ViewBag.Departments = new SelectList(await _departmentService.GetAllAsync(), "DepartmentId", "DepartmentName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateViewModel model)
        {
            var user = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (user == null || user.Role != "Admin")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    CourseName = model.CourseName,
                    Credits = model.Credits,
                    DepartmentId = model.DepartmentId
                };

                await _courseManagementService.AddAsync(course);
                await _courseManagementService.SaveAsync();
                TempData["SuccessMessage"] = "Course created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(await _departmentService.GetAllAsync(), "DepartmentId", "DepartmentName", model.DepartmentId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (user == null || user.Role != "Admin")
                return RedirectToAction("Login", "Account");

            var course = await _courseManagementService.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            var model = new CourseEditViewModel
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                Credits = course.Credits,
                DepartmentId = course.DepartmentId
            };

            ViewBag.Departments = new SelectList(await _departmentService.GetAllAsync(), "DepartmentId", "DepartmentName", model.DepartmentId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseEditViewModel model)
        {
            var user = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (user == null || user.Role != "Admin")
                return RedirectToAction("Login", "Account");

            if (id != model.CourseId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var course = await _courseManagementService.GetByIdAsync(id);
                if (course == null)
                    return NotFound();

                course.CourseName = model.CourseName;
                course.Credits = model.Credits;
                course.DepartmentId = model.DepartmentId;

                await _courseManagementService.UpdateAsync(course);
                await _courseManagementService.SaveAsync();
                TempData["SuccessMessage"] = "Course updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(await _departmentService.GetAllAsync(), "DepartmentId", "DepartmentName", model.DepartmentId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (user == null || user.Role != "Admin")
                return RedirectToAction("Login", "Account");

            var course = await _courseManagementService.GetCourseByIdWithDetailsAsync(id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (user == null || user.Role != "Admin")
                return RedirectToAction("Login", "Account");

            await _courseManagementService.DeleteAsync(id);
            await _courseManagementService.SaveAsync();
            TempData["SuccessMessage"] = "Course deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        public class CourseViewModel
        {
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public int Credits { get; set; }
            public string DepartmentName { get; set; }
        }

        public class CourseCreateViewModel
        {
            [Required]
            public string CourseName { get; set; }

            [Required]
            [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10.")]
            public int Credits { get; set; }

            [Required]
            public int DepartmentId { get; set; }
        }

        public class CourseEditViewModel
        {
            public int CourseId { get; set; }

            [Required]
            public string CourseName { get; set; }

            [Required]
            [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10.")]
            public int Credits { get; set; }

            [Required]
            public int DepartmentId { get; set; }
        }
    }
}