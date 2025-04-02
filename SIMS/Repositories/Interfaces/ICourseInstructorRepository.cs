using SIMS.Models;

namespace SIMS.Repositories.Interfaces
{
    public interface ICourseInstructorRepository : IRepository<CourseInstructor>
    {
        Task<List<CourseInstructor>> GetCoursesByInstructorIdAsync(int instructorId);
        Task<CourseInstructor> GetCourseInstructorAsync(int instructorId, int courseId);
    }
}
