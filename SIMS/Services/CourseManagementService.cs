using SIMS.Models;
using SIMS.Repositories.Interfaces;
using SIMS.Services.Interfaces;

namespace SIMS.Services
{
    public class CourseManagementService : BaseService<Course>, ICourseManagementService
    {
        private readonly ICourseRepository _courseRepository;
        public CourseManagementService(ICourseRepository courseRepository) : base(courseRepository) 
        {
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<Course>> GetCourseWithDepartmentAsync()
        {
            return await _courseRepository.GetCourseWithDepartmentAsync();
        }

        public async Task<Course> GetCourseByIdWithDetailsAsync(int courseId)
        {
            return await _courseRepository.GetCourseWithDetailsAsync(courseId);
        }

    }
}
