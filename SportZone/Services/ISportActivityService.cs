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
    
    // New search and filter methods
    Task<IEnumerable<SportActivity>> SearchActivitiesByLocationAsync(double latitude, double longitude, double radiusKm);
    Task<IEnumerable<SportActivity>> SearchActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<SportActivity>> SearchActivitiesWithFiltersAsync(
        SportType? sportType = null,
        double? latitude = null,
        double? longitude = null,
        double? radiusKm = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        bool? hasAvailableSlots = null,
        bool? isActive = true
    );
}
