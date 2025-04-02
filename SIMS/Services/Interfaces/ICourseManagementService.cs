using SIMS.Models;

namespace SIMS.Services.Interfaces
{
    public interface ICourseManagementService : IService<Course>
    {
        Task<IEnumerable<Course>> GetCourseWithDepartmentAsync();

        Task<Course> GetCourseByIdWithDetailsAsync(int courseId);
    }
}
