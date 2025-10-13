using SportZone.Models;

namespace SportZone.Repositories;

public interface ISportActivityRepository
{
    Task<SportActivity> CreateAsync(SportActivity sportActivity);
    Task<SportActivity?> GetByIdAsync(string id);
    Task<SportActivity?> GetByUniqueIdAsync(string uniqueId);
    Task<IEnumerable<SportActivity>> GetAllAsync();
    Task<IEnumerable<SportActivity>> GetByUserIdAsync(string userId);
    Task<IEnumerable<SportActivity>> GetBySportTypeAsync(SportType sportType);
    Task<IEnumerable<SportActivity>> GetActiveActivitiesAsync();
    Task<bool> UpdateAsync(string id, SportActivity sportActivity);
    Task<bool> DeleteAsync(string id);
    Task<bool> JoinActivityAsync(string activityId, string userId);
    Task<bool> LeaveActivityAsync(string activityId, string userId);
}
