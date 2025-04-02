using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Models;
using SIMS.Repositories.Interfaces;

namespace SIMS.Repositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(MyDbContext context) : base(context) { }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
            => await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == email);


        public async Task<List<ApplicationUser>> GetUsersWithDepartmentAsync()
        {
            return await _context.ApplicationUsers
                .Where(u => u.Role == "Student")
                .Include(u => u.Department)
                .ToListAsync();
        }
    }
}
