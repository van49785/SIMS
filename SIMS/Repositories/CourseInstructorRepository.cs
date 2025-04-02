using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Models;
using SIMS.Repositories.Interfaces;

namespace SIMS.Repositories
{
    public class CourseInstructorRepository : Repository<CourseInstructor>, ICourseInstructorRepository
    {
        public CourseInstructorRepository(MyDbContext context) : base(context) 
        {
        }

        public async Task<List<CourseInstructor>> GetCoursesByInstructorIdAsync(int instructorId)
        {
            return await _context.CourseInstructors
                .Include(ci => ci.Course)
                .Include(ci => ci.Instructor)
                .Where(ci => ci.InstructorId == instructorId)
                .ToListAsync();
        }

        public async Task<CourseInstructor> GetCourseInstructorAsync(int instructorId, int courseId)
        {
            return await _context.CourseInstructors
                .Include(ci => ci.Course)
                .FirstOrDefaultAsync(ci => ci.InstructorId == instructorId && ci.CourseId == courseId);
        }
    }
}
