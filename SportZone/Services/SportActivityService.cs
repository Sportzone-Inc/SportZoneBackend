using SportZone.Models;
using SportZone.Repositories;

namespace SportZone.Services;

public class SportActivityService : ISportActivityService
{
    private readonly ISportActivityRepository _sportActivityRepository;
    private readonly IUserRepository _userRepository;

    public SportActivityService(ISportActivityRepository sportActivityRepository, IUserRepository userRepository)
    {
        _sportActivityRepository = sportActivityRepository;
        _userRepository = userRepository;
    }

    public async Task<SportActivity> CreateSportActivityAsync(SportActivity sportActivity)
    {
        // Verify that the user exists
        var user = await _userRepository.GetByIdAsync(sportActivity.CreatedBy);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {sportActivity.CreatedBy} not found");
        }

        return await _sportActivityRepository.CreateAsync(sportActivity);
    }

    public async Task<SportActivity?> GetSportActivityByIdAsync(string id)
    {
        return await _sportActivityRepository.GetByIdAsync(id);
    }

    public async Task<SportActivity?> GetSportActivityByUniqueIdAsync(string uniqueId)
    {
        return await _sportActivityRepository.GetByUniqueIdAsync(uniqueId);
    }

    public async Task<IEnumerable<SportActivity>> GetAllSportActivitiesAsync()
    {
        return await _sportActivityRepository.GetAllAsync();
    }

    public async Task<IEnumerable<SportActivity>> GetSportActivitiesByUserAsync(string userId)
    {
        return await _sportActivityRepository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<SportActivity>> GetSportActivitiesByTypeAsync(SportType sportType)
    {
        return await _sportActivityRepository.GetBySportTypeAsync(sportType);
    }

    public async Task<IEnumerable<SportActivity>> GetActiveSportActivitiesAsync()
    {
        return await _sportActivityRepository.GetActiveActivitiesAsync();
    }

    public async Task<bool> UpdateSportActivityAsync(string id, SportActivity sportActivity)
    {
        return await _sportActivityRepository.UpdateAsync(id, sportActivity);
    }

    public async Task<bool> DeleteSportActivityAsync(string id)
    {
        return await _sportActivityRepository.DeleteAsync(id);
    }

    public async Task<bool> JoinSportActivityAsync(string activityId, string userId)
    {
        var activity = await _sportActivityRepository.GetByIdAsync(activityId);
        if (activity == null)
        {
            return false;
        }

        if (activity.MaxParticipants.HasValue && activity.CurrentParticipants >= activity.MaxParticipants.Value)
        {
            throw new InvalidOperationException("Activity is full");
        }

        if (activity.Participants.Contains(userId))
        {
            throw new InvalidOperationException("User already joined this activity");
        }

        return await _sportActivityRepository.JoinActivityAsync(activityId, userId);
    }

    public async Task<bool> LeaveSportActivityAsync(string activityId, string userId)
    {
        var activity = await _sportActivityRepository.GetByIdAsync(activityId);
        if (activity == null)
        {
            return false;
        }

        if (!activity.Participants.Contains(userId))
        {
            throw new InvalidOperationException("User is not part of this activity");
        }

        return await _sportActivityRepository.LeaveActivityAsync(activityId, userId);
    }

    public async Task<IEnumerable<SportActivity>> SearchActivitiesByLocationAsync(double latitude, double longitude, double radiusKm)
    {
        var allActivities = await _sportActivityRepository.GetActiveActivitiesAsync();
        
        return allActivities.Where(activity =>
            activity.Latitude.HasValue &&
            activity.Longitude.HasValue &&
            CalculateDistance(latitude, longitude, activity.Latitude.Value, activity.Longitude.Value) <= radiusKm
        );
    }

    public async Task<IEnumerable<SportActivity>> SearchActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _sportActivityRepository.GetActivitiesByDateRangeAsync(startDate, endDate);
    }

    public async Task<IEnumerable<SportActivity>> SearchActivitiesWithFiltersAsync(
        SportType? sportType = null,
        double? latitude = null,
        double? longitude = null,
        double? radiusKm = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        bool? hasAvailableSlots = null,
        bool? isActive = true)
    {
        IEnumerable<SportActivity> activities;

        // Start with sport type filter if provided
        if (sportType.HasValue)
        {
            activities = await _sportActivityRepository.GetBySportTypeAsync(sportType.Value);
        }
        else
        {
            activities = await _sportActivityRepository.GetAllAsync();
        }

        // Apply active filter
        if (isActive.HasValue)
        {
            activities = activities.Where(a => a.IsActive == isActive.Value);
        }

        // Apply location filter
        if (latitude.HasValue && longitude.HasValue && radiusKm.HasValue)
        {
            activities = activities.Where(activity =>
                activity.Latitude.HasValue &&
                activity.Longitude.HasValue &&
                CalculateDistance(latitude.Value, longitude.Value, activity.Latitude.Value, activity.Longitude.Value) <= radiusKm.Value
            );
        }

        // Apply date range filter
        if (startDate.HasValue && endDate.HasValue)
        {
            activities = activities.Where(activity =>
                activity.ScheduledDate.HasValue &&
                activity.ScheduledDate.Value >= startDate.Value &&
                activity.ScheduledDate.Value <= endDate.Value
            );
        }
        else if (startDate.HasValue)
        {
            activities = activities.Where(activity =>
                activity.ScheduledDate.HasValue &&
                activity.ScheduledDate.Value >= startDate.Value
            );
        }
        else if (endDate.HasValue)
        {
            activities = activities.Where(activity =>
                activity.ScheduledDate.HasValue &&
                activity.ScheduledDate.Value <= endDate.Value
            );
        }

        // Apply available slots filter
        if (hasAvailableSlots.HasValue && hasAvailableSlots.Value)
        {
            activities = activities.Where(activity =>
                !activity.MaxParticipants.HasValue ||
                activity.CurrentParticipants < activity.MaxParticipants.Value
            );
        }

        return activities;
    }

    /// <summary>
    /// Calculate distance between two GPS coordinates using Haversine formula
    /// </summary>
    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
