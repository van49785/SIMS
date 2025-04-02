using SIMS.Models;
using SIMS.Repositories.Interfaces;
using SIMS.Services.Interfaces;

namespace SIMS.Services
{
    public class CourseService : BaseService<Course>, ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        public CourseService(ICourseRepository courseRepository) : base(courseRepository) { }
    }
}
