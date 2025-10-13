using SportZone.Models;

namespace SportZone.Repositories;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> UpdateAsync(string id, User user);
    Task<bool> DeleteAsync(string id);
}
