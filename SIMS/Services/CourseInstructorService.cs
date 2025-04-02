using SIMS.Models;
using SIMS.Repositories.Interfaces;
using SIMS.Services.Interfaces;

namespace SIMS.Services
{
    public class CourseInstructorService : BaseService<CourseInstructor>, ICourseInstructorService
    {
        private readonly ICourseInstructorRepository _courseInstructorRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IApplicationUserRepository _userRepository;

        public CourseInstructorService(
            ICourseInstructorRepository courseInstructorRepository,
            IEnrollmentRepository enrollmentRepository,
            IApplicationUserRepository userRepository)
            : base(courseInstructorRepository)
        {
            _courseInstructorRepository = courseInstructorRepository;
            _enrollmentRepository = enrollmentRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Course>> GetCoursesByInstructorAsync(int instructorId)
        {
            var instructor = await _userRepository.GetByIdAsync(instructorId);
            if (instructor == null || instructor.Role != "Instructor")
                throw new InvalidOperationException("User is not an instructor.");

            var courseInstructors = await _courseInstructorRepository.GetCoursesByInstructorIdAsync(instructorId);
            return courseInstructors.Select(ci => ci.Course).ToList();
        }

        public async Task<(Course Course, List<Enrollment> Enrollments)> GetCourseEnrollmentAsync(int instructorId, int courseId)
        {
            var instructor = await _userRepository.GetByIdAsync(instructorId);
            if (instructor == null || instructor.Role != "Instructor")
                throw new InvalidOperationException("User is not an instructor.");

            var courseInstructor = await _courseInstructorRepository.GetCourseInstructorAsync(instructorId, courseId);
            if (courseInstructor == null)
                throw new UnauthorizedAccessException("Instructor is not assigned to this course.");

            var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId);
            return (courseInstructor.Course, enrollments);
        }

        public async Task UpdateGradeAsync(int instructorId, int studentId, int courseId, float? grade)
        {
            if (grade.HasValue && (grade < 0 || grade > 10))
                throw new ArgumentException("Grade must be between 0 and 10.");

            var instructor = await _userRepository.GetByIdAsync(instructorId);
            if (instructor == null || instructor.Role != "Instructor")
                throw new InvalidOperationException("User is not an instructor.");

            var courseInstructor = await _courseInstructorRepository.GetCourseInstructorAsync(instructorId, courseId);
            if (courseInstructor == null)
                throw new UnauthorizedAccessException("Instructor is not assigned to this course.");

            var enrollment = await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId);
            var studentEnrollment = enrollment.FirstOrDefault(e => e.UserId == studentId);
            if (studentEnrollment == null)
                throw new KeyNotFoundException("Student enrollment not found.");

            studentEnrollment.Grade = grade;
            studentEnrollment.Status = grade.HasValue ? (grade >= 4 ? "Completed" : "Failed") : "Enrolled";

            await _enrollmentRepository.UpdateAsync(studentEnrollment);
            await _enrollmentRepository.SaveAsync();
        }

    }
}