using SportZone.Models;

namespace SportZone.Services;

public interface ISportActivityService
{
    Task<SportActivity> CreateSportActivityAsync(SportActivity sportActivity);
    Task<SportActivity?> GetSportActivityByIdAsync(string id);
    Task<SportActivity?> GetSportActivityByUniqueIdAsync(string uniqueId);
    Task<IEnumerable<SportActivity>> GetAllSportActivitiesAsync();
    Task<IEnumerable<SportActivity>> GetSportActivitiesByUserAsync(string userId);
    Task<IEnumerable<SportActivity>> GetSportActivitiesByTypeAsync(SportType sportType);
    Task<IEnumerable<SportActivity>> GetActiveSportActivitiesAsync();
    Task<bool> UpdateSportActivityAsync(string id, SportActivity sportActivity);
    Task<bool> DeleteSportActivityAsync(string id);
    Task<bool> JoinSportActivityAsync(string activityId, string userId);
    Task<bool> LeaveSportActivityAsync(string activityId, string userId);
}
