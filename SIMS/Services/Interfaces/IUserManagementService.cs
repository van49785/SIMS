using SIMS.Models;
using SIMS.Controllers;

namespace SIMS.Services.Interfaces
{
    public interface IUserManagementService : IService<ApplicationUser>
    {
        Task<(bool Success, string ErrorMessage)> AddUserAsync(ApplicationUser user); 
        Task<(bool Success, string ErrorMessage)> UpdateUserAsync(ApplicationUser user); 
        Task<(bool Success, string ErrorMessage)> DeleteUserAsync(int userId);
        Task<bool> LoginAsync(string email, string password);
        Task<ApplicationUser> GetCurrentUserAsync(HttpContext httpContext);
        Task<ApplicationUser> GetByEmailAsync(string email);
    }
}
