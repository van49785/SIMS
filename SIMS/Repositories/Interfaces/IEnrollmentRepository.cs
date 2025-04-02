using SIMS.Models;

namespace SIMS.Repositories.Interfaces
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<List<Enrollment>> GetEnrollmentsByStudentAsync(int userId);
        Task<List<Enrollment>> GetEnrollmentsByCourseAsync(int courseId);
    }
}
