using SIMS.Models;

namespace SIMS.Repositories.Interfaces
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<List<ApplicationUser>> GetUsersWithDepartmentAsync();
    }
}
