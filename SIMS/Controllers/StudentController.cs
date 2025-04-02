using Microsoft.AspNetCore.Mvc;
using SIMS.Models;
using SIMS.Repositories.Interfaces;
using SIMS.Services.Interfaces;

namespace SIMS.Controllers
{
    public class StudentController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IEnrollmentService _enrollmentRepository;

        public StudentController(
            IUserManagementService userManagementService,
            IEnrollmentService enrollmentRepository)
        {
            _userManagementService = userManagementService;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var student = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (student == null || student.Role != "Student")
                return RedirectToAction("Login", "Account");

            return View(student);
        }

        public async Task<IActionResult> MyCourses(string filterStatus = null)
        {
            var student = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (student == null || student.Role != "Student")
                return RedirectToAction("Login", "Account");

            var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentAsync(student.Id);
            if (!string.IsNullOrEmpty(filterStatus))
                enrollments = enrollments.Where(e => e.Status == filterStatus).ToList();

            var model = new StudentCoursesViewModel
            {
                Courses = enrollments.Select(e => new CourseViewModel
                {
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    Credits = e.Course.Credits,
                    Status = e.Status
                }).ToList()
            };

            ViewBag.FilterStatus = filterStatus;
            return View(model);
        }

        public async Task<IActionResult> MyGrades()
        {
            var student = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (student == null || student.Role != "Student")
                return RedirectToAction("Login", "Account");

            var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentAsync(student.Id);
            var model = new StudentGradesViewModel
            {
                Grades = enrollments.Select(e => new GradeViewModel
                {
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    Credits = e.Course.Credits,
                    Grade = e.Grade,
                    Status = e.Status
                }).ToList()
            };

            return View(model);
        }

        public class StudentCoursesViewModel
        {
            public List<CourseViewModel> Courses { get; set; }
        }

        public class CourseViewModel
        {
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public int Credits { get; set; }
            public string Status { get; set; }
        }

        public class StudentGradesViewModel
        {
            public List<GradeViewModel> Grades { get; set; }
        }

        public class GradeViewModel
        {
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public int Credits { get; set; }
            public float? Grade { get; set; }
            public string Status { get; set; }
        }
    }
}