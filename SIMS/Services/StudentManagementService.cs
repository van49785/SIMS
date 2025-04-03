using SIMS.Controllers;
using SIMS.Models;
using SIMS.Repositories.Interfaces;
using SIMS.Services.Interfaces;
using System.Transactions;

namespace SIMS.Services
{
    public class StudentManagementService : BaseService<ApplicationUser>, IStudentManagementService
    {
        private readonly IApplicationUserRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public StudentManagementService(IApplicationUserRepository studentRepository, IEnrollmentRepository enrollmentRepository)
            : base(studentRepository)
        {
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<ApplicationUser>> GetStudentsWithDepartmentAsync()
        {
            var allUsers = await _studentRepository.GetUsersWithDepartmentAsync();
            return allUsers.Where(u => u.Role == "Student").ToList();
        }

        public async Task EnrollStudentAsync(int userId, int courseId)
        {
            var student = await _studentRepository.GetByIdAsync(userId);
            if (student == null || student.Role != "Student")
                throw new InvalidOperationException("User is not a student.");

            var existing = await _enrollmentRepository.GetEnrollmentsByStudentAsync(userId);
            if (existing.Any(e => e.CourseId == courseId))
                throw new InvalidOperationException("Student is already enrolled in this course.");

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                Grade = null,
                Status = "Enrolled"
            };

            await _enrollmentRepository.AddAsync(enrollment);
            await _enrollmentRepository.SaveAsync();
        }

        public async Task<List<ApplicationUser>> GetAllStudentsAsync()
        {
            var allUsers = await _studentRepository.GetUsersWithDepartmentAsync();
            var students = allUsers.Where(u => u.Role == "Student").ToList();
            return students;
        }

        public async Task AssignCoursesAsync(int userId, List<int> courseIds)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var student = await _studentRepository.GetByIdAsync(userId);
                    if (student == null || student.Role != "Student")
                        throw new InvalidOperationException("User is not a student.");

                    var existingEnrollments = await _enrollmentRepository.GetEnrollmentsByStudentAsync(userId);

                    foreach (var enrollment in existingEnrollments)
                        await _enrollmentRepository.DeleteAsync(enrollment.EnrollmentId);

                    await _enrollmentRepository.SaveAsync();

                    if (courseIds != null && courseIds.Any())
                    {
                        foreach (var courseId in courseIds)
                        {
                            var enrollment = new Enrollment
                            {
                                UserId = userId,
                                CourseId = courseId,
                                Grade = null,
                                Status = "Enrolled"
                            };
                            await _enrollmentRepository.AddAsync(enrollment);
                        }

                        await _enrollmentRepository.SaveAsync();
                    }

                    scope.Complete();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}