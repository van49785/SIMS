using SIMS.Models;

namespace SIMS.Services.Interfaces
{
    public interface IEnrollmentService : IService<Enrollment>
    {
        Task<List<Enrollment>> GetEnrollmentsByStudentAsync(int userId);
    }
}
