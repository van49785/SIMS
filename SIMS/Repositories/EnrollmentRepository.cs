using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Models;
using SIMS.Repositories.Interfaces;

namespace SIMS.Repositories
{
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository (MyDbContext context) : base (context) { }

        public async Task<List<Enrollment>> GetEnrollmentsByStudentAsync(int userId)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Enrollment>> GetEnrollmentsByCourseAsync(int courseId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }
    }
}
