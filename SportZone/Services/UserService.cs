using SportZone.Models;
using SportZone.Repositories;

namespace SportZone.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        // Hash the password before storing
        user.Password = _passwordHasher.HashPassword(user.Password);
        return await _userRepository.CreateAsync(user);
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<bool> UpdateUserAsync(string id, User user)
    {
        // Hash the password if it's being updated
        if (!string.IsNullOrEmpty(user.Password))
        {
            user.Password = _passwordHasher.HashPassword(user.Password);
        }
        return await _userRepository.UpdateAsync(id, user);
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return false;
        }

        return _passwordHasher.VerifyPassword(password, user.Password);
    }
}
