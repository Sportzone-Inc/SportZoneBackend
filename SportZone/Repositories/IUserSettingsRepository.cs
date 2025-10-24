using SportZone.Models;

namespace SportZone.Repositories;

public interface IUserSettingsRepository
{
    Task<UserSettings> CreateAsync(UserSettings settings);
    Task<UserSettings?> GetByIdAsync(string id);
    Task<UserSettings?> GetByUserIdAsync(string userId);
    Task<bool> UpdateAsync(string id, UserSettings settings);
    Task<bool> DeleteAsync(string id);
}
