using SportZone.Models;

namespace SportZone.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(User user);
    Task<User?> GetUserByIdAsync(string id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(string id, User user);
    Task<bool> DeleteUserAsync(string id);
    Task<bool> ValidateUserCredentialsAsync(string email, string password);
}
