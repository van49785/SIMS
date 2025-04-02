using SIMS.Controllers;
using SIMS.Models;

namespace SIMS.Services.Interfaces
{
    public interface IStudentManagementService : IService<ApplicationUser>
    {
        Task EnrollStudentAsync(int userId, int courseId);
        Task<List<ApplicationUser>> GetAllStudentsAsync();
        Task AssignCoursesAsync(int userId, List<int> courseIds);
        Task<IEnumerable<ApplicationUser>> GetStudentsWithDepartmentAsync();
    }
}
