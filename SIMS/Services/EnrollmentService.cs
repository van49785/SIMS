using SIMS.Models;
using SIMS.Repositories.Interfaces;
using SIMS.Services.Interfaces;

namespace SIMS.Services
{
    public class EnrollmentService : BaseService<Enrollment>, IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository) : base(enrollmentRepository) 
        { 
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<List<Enrollment>> GetEnrollmentsByStudentAsync(int userId)
        {
            return (await _enrollmentRepository.GetEnrollmentsByStudentAsync(userId))
                .ToList();
        }
    }
}
