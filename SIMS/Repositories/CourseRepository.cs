using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Models;
using SIMS.Repositories.Interfaces;

namespace SIMS.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(MyDbContext context) : base(context) { }

        public async Task<Course> GetCourseWithDetailsAsync(int courseId)
        {
            return await _context.Courses
            .Include(c => c.Department)
            .Include(c => c.Enrollments).ThenInclude(e => e.Student)
            .Include(c => c.CourseInstructors).ThenInclude(ci => ci.Instructor)
            .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<IEnumerable<Course>> GetCourseWithDepartmentAsync()
        {
            return await _context.Courses
                .Include(c => c.Department)
                .ToListAsync();
        }
    }
}
