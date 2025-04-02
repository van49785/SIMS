using SIMS.Models;
using SIMS.Repositories.Interfaces;
using SIMS.Services.Interfaces;

namespace SIMS.Services
{
    public class UserManagementService : BaseService<ApplicationUser>, IUserManagementService
    {
        private readonly IApplicationUserRepository _userRepository;

        public UserManagementService(IApplicationUserRepository userRepository) : base(userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<(bool Success, string ErrorMessage)> AddUserAsync(ApplicationUser user)
        {
            if (user == null) return (false, "Invalid user data");
            if (string.IsNullOrWhiteSpace(user.Email)) return (false, "Email is required.");
            if (string.IsNullOrWhiteSpace(user.Password)) return (false, "Password is required.");

            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return (false, "Email already exists.");
            }

            await _repository.AddAsync(user);
            await _repository.SaveAsync();
            return (true, null);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateUserAsync(ApplicationUser user)
        {
            if (user == null) return (false, "Invalid user data.");

            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
                return (false, "User not found.");

            existingUser.UserCode = user.UserCode;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.DepartmentId = user.DepartmentId;
            existingUser.Role = user.Role;

            await _repository.UpdateAsync(existingUser);
            await _repository.SaveAsync();
            return (true, null);
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
            => await _userRepository.GetByEmailAsync(email);

        public async Task<(bool Success, string ErrorMessage)> DeleteUserAsync(int userId)
        {
            try
            {
                await _repository.DeleteAsync(userId);
                await _repository.SaveAsync();
                return (true, null);
            }
            catch (KeyNotFoundException)
            {
                return (false, "User not found.");
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return false;

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || password != user.Password)
                return false;

            return true;
        }

        public async Task<ApplicationUser> GetCurrentUserAsync(HttpContext httpContext)
        {
            var userIdString = httpContext.Session.GetString("UserId");
            if (int.TryParse(userIdString, out int userId))
            {
                return await _userRepository.GetByIdAsync(userId);
            }
            return null;
        }
    }
}