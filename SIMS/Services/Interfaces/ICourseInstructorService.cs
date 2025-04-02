using SIMS.Controllers;
using SIMS.Models;

namespace SIMS.Services.Interfaces
{
    public interface ICourseInstructorService : IService<CourseInstructor>
    {
        Task<List<Course>> GetCoursesByInstructorAsync(int instructorId);
        Task<(Course Course, List<Enrollment> Enrollments)> GetCourseEnrollmentAsync(int instructorId, int courseId);
        Task UpdateGradeAsync(int instructorId, int studentId, int courseId, float? grade);
    }
}
