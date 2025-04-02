using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIMS.Models;
using SIMS.Repositories.Interfaces;
using SIMS.Services;
using SIMS.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SIMS.Controllers
{
    public class StudentManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IStudentManagementService _studentManagementService;
        private readonly ICourseService _courseService;
        private readonly IDepartmentService _departmentService;
        private readonly IEnrollmentService _enrollmentService;

        public StudentManagementController(
            IUserManagementService userManagementService,
            IStudentManagementService studentManagementService,
            ICourseService courseService,
            IDepartmentService departmentService,
            IEnrollmentService enrollmentService)
        {
            _userManagementService = userManagementService;
            _studentManagementService = studentManagementService;
            _courseService = courseService;
            _departmentService = departmentService;
            _enrollmentService = enrollmentService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            var students = await _studentManagementService.GetAllStudentsAsync();
            var studentViewModels = new List<StudentViewModel>();

            foreach (var student in students)
            {
                var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(student.Id);
                studentViewModels.Add(new StudentViewModel
                {
                    Id = student.Id,
                    Email = student.Email,
                    UserCode = student.UserCode,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    DepartmentName = student.Department?.DepartmentName ?? "N/A",
                    EnrolledCourses = enrollments.Select(e => e.Course.CourseName).ToList() ?? new List<string>()
                });
            }

            return View(studentViewModels);
        }

        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            ViewBag.Departments = new SelectList(await _departmentService.GetAllAsync(), "DepartmentId", "DepartmentName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCreateViewModel model)
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser
                {
                    Email = model.Email,
                    UserCode = model.UserCode,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = model.Password, // Lưu ý: Cần mã hóa mật khẩu trước khi lưu
                    DepartmentId = model.DepartmentId,
                    Role = "Student"
                };

                var (success, error) = await _userManagementService.AddUserAsync(applicationUser);
                if (success)
                {
                    TempData["SuccessMessage"] = "Student created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", error ?? "Failed to create student.");
            }

            ViewBag.Departments = new SelectList(await _departmentService.GetAllAsync(), "DepartmentId", "DepartmentName", model.DepartmentId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            var student = await _userManagementService.GetByIdAsync(id);
            if (student == null || student.Role != "Student")
                return NotFound();

            var model = new StudentEditViewModel
            {
                Id = student.Id,
                Email = student.Email,
                UserCode = student.UserCode,
                FirstName = student.FirstName,
                LastName = student.LastName,
                DepartmentId = student.DepartmentId
            };

            ViewBag.Departments = new SelectList(await _departmentService.GetAllAsync(), "DepartmentId", "DepartmentName", model.DepartmentId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentEditViewModel model)
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existingUser = await _userManagementService.GetByIdAsync(model.Id);
                if (existingUser == null)
                    return NotFound();

                // Cập nhật các thuộc tính của existingUser từ model
                existingUser.Email = model.Email;
                existingUser.UserCode = model.UserCode;
                existingUser.FirstName = model.FirstName;
                existingUser.LastName = model.LastName;
                existingUser.DepartmentId = model.DepartmentId;
                // Không thay đổi mật khẩu hoặc Role

                // Gọi hàm cập nhật
                var (success, error) = await _userManagementService.UpdateUserAsync(existingUser);
                if (success)
                {
                    TempData["SuccessMessage"] = "Student updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", error ?? "Failed to update student.");
            }

            ViewBag.Departments = new SelectList(await _departmentService.GetAllAsync(), "DepartmentId", "DepartmentName", model.DepartmentId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            var student = await _userManagementService.GetByIdAsync(id);
            if (student == null || student.Role != "Student")
                return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            // Kiểm tra xem sinh viên tồn tại không
            var student = await _userManagementService.GetByIdAsync(id);
            if (student == null || student.Role != "Student")
                return NotFound();

            try
            {
                // Delete enrollment before deleting student
                var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(id);
                foreach (var enrollment in enrollments)
                {
                    await _enrollmentService.DeleteAsync(enrollment.EnrollmentId);
                }

                // Lưu các thay đổi xóa enrollment
                await _enrollmentService.SaveAsync();

                var (success, error) = await _userManagementService.DeleteUserAsync(id);
                if (success)
                    TempData["SuccessMessage"] = "Student deleted successfully!";
                else
                    TempData["ErrorMessage"] = error ?? "Failed to delete student.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting student: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AssignCourses(int id)
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            var student = await _userManagementService.GetByIdAsync(id);
            if (student == null || student.Role != "Student")
                return NotFound();

            var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(id);
            var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToList();
            var courses = await _courseService.GetAllAsync();

            var model = new AssignCoursesViewModel
            {
                StudentId = id,
                StudentName = $"{student.FirstName} {student.LastName}",
                AvailableCourses = courses.Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = c.CourseName
                }).ToList(),
                EnrolledCourseIds = enrolledCourseIds,
                SelectedCourseIds = enrolledCourseIds.ToArray()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignCourses(int id, AssignCoursesViewModel model)
        {
            var currentUser = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (currentUser == null || currentUser.Role != "Admin")
                return RedirectToAction("Login", "Account");

            var student = await _userManagementService.GetByIdAsync(id);
            if (student == null || student.Role != "Student")
                return NotFound();

            try
            {
                await _studentManagementService.AssignCoursesAsync(id, model.SelectedCourseIds?.ToList() ?? new List<int>());
                TempData["SuccessMessage"] = "Courses assigned successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var courses = await _courseService.GetAllAsync();
                var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(id);
                var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToList();

                model.StudentName = $"{student.FirstName} {student.LastName}";
                model.AvailableCourses = courses.Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = c.CourseName
                }).ToList();
                model.EnrolledCourseIds = enrolledCourseIds;

                return View(model);
            }
        }

        public class StudentViewModel
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string UserCode { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string DepartmentName { get; set; }
            public List<string> EnrolledCourses { get; set; }
        }

        public class StudentCreateViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [StringLength(10)]
            public string UserCode { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            public int? DepartmentId { get; set; }
        }

        public class StudentEditViewModel
        {
            public int Id { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [StringLength(10)]
            public string UserCode { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            public int? DepartmentId { get; set; }
        }

        public class AssignCoursesViewModel
        {
            public int StudentId { get; set; }
            public string StudentName { get; set; }
            public List<SelectListItem> AvailableCourses { get; set; }
            public List<int> EnrolledCourseIds { get; set; }
            public int[] SelectedCourseIds { get; set; }
        }
    }
}