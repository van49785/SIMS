using Microsoft.AspNetCore.Mvc;
using SIMS.Models;
using SIMS.Services.Interfaces;

namespace SIMS.Controllers
{
    public class CourseInstructorController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ICourseInstructorService _courseInstructorService;

        public CourseInstructorController(
            IUserManagementService userManagementService,
            ICourseInstructorService courseInstructorService)
        {
            _userManagementService = userManagementService;
            _courseInstructorService = courseInstructorService;
        }

        public async Task<IActionResult> Index()
        {
            var instructor = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (instructor == null || instructor.Role != "Instructor")
                return RedirectToAction("Login", "Account");

            var courses = await _courseInstructorService.GetCoursesByInstructorAsync(instructor.Id);
            var model = new InstructorCoursesViewModel
            {
                Courses = courses.Select(c => new CourseViewModel
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Credits = c.Credits
                }).ToList()
            };
            return View(model);
        }

        public async Task<IActionResult> ManageGrades(int courseId)
        {
            var instructor = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (instructor == null || instructor.Role != "Instructor")
                return RedirectToAction("Login", "Account");

            try
            {
                var (course, enrollments) = await _courseInstructorService.GetCourseEnrollmentAsync(instructor.Id, courseId);
                var model = new ManageGradesViewModel
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    StudentGrades = enrollments.Select(e => new StudentGradeViewModel
                    {
                        StudentId = e.UserId,
                        StudentName = $"{e.Student.FirstName} {e.Student.LastName}",
                        UserCode = e.Student.UserCode,
                        Grade = e.Grade
                    }).ToList()
                };
                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGrade(int studentId, int courseId, float? grade)
        {
            var instructor = await _userManagementService.GetCurrentUserAsync(HttpContext);
            if (instructor == null || instructor.Role != "Instructor")
                return RedirectToAction("Login", "Account");

            try
            {
                await _courseInstructorService.UpdateGradeAsync(instructor.Id, studentId, courseId, grade);
                TempData["SuccessMessage"] = "Grade updated successfully!";
                return RedirectToAction("ManageGrades", new { courseId });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("ManageGrades", new { courseId });
            }
        }

        // View Models nội bộ trong Controller
        public class InstructorCoursesViewModel
        {
            public List<CourseViewModel> Courses { get; set; }
        }

        public class CourseViewModel
        {
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public int Credits { get; set; }
        }

        public class ManageGradesViewModel
        {
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public List<StudentGradeViewModel> StudentGrades { get; set; }
        }

        public class StudentGradeViewModel
        {
            public int StudentId { get; set; }
            public string StudentName { get; set; }
            public string UserCode { get; set; }
            public float? Grade { get; set; }
        }
    }
}