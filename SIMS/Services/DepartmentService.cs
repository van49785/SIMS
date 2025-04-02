using SIMS.Models;
using SIMS.Repositories.Interfaces;
using SIMS.Services.Interfaces;

namespace SIMS.Services
{
    public class DepartmentService : BaseService<Department>, IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
            : base(departmentRepository)
        {
        }
    }
}
