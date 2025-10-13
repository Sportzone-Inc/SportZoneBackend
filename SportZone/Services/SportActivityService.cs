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
}
