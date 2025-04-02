using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Models;
using SIMS.Repositories.Interfaces;

namespace SIMS.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(MyDbContext dbContext) : base(dbContext) { }
    }
}
