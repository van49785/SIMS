using SIMS.Models;

namespace SIMS.Repositories.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course> GetCourseWithDetailsAsync(int courseId);
        Task<IEnumerable<Course>> GetCourseWithDepartmentAsync();
    }
}
