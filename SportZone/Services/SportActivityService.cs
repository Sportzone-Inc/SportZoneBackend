using SportZone.Models;
using SportZone.Repositories;

namespace SportZone.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _EventRepository;
    private readonly IUserRepository _userRepository;

    public EventService(IEventRepository EventRepository, IUserRepository userRepository)
    {
        _EventRepository = EventRepository;
        _userRepository = userRepository;
    }

    public async Task<Event> CreateEventAsync(Event Event)
    {
        // Verify that the user exists
        var user = await _userRepository.GetByIdAsync(Event.CreatedBy);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {Event.CreatedBy} not found");
        }

        return await _EventRepository.CreateAsync(Event);
    }

    public async Task<Event?> GetEventByIdAsync(string id)
    {
        return await _EventRepository.GetByIdAsync(id);
    }

    public async Task<Event?> GetEventByUniqueIdAsync(string uniqueId)
    {
        return await _EventRepository.GetByUniqueIdAsync(uniqueId);
    }

    public async Task<IEnumerable<Event>> GetAlleventsAsync()
    {
        return await _EventRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Event>> GeteventsByUserAsync(string userId)
    {
        return await _EventRepository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Event>> GeteventsByTypeAsync(SportType sportType)
    {
        return await _EventRepository.GetBySportTypeAsync(sportType);
    }

    public async Task<IEnumerable<Event>> GetActiveeventsAsync()
    {
        return await _EventRepository.GetActiveActivitiesAsync();
    }

    public async Task<bool> UpdateEventAsync(string id, Event Event)
    {
        return await _EventRepository.UpdateAsync(id, Event);
    }

    public async Task<bool> DeleteEventAsync(string id)
    {
        return await _EventRepository.DeleteAsync(id);
    }

    public async Task<bool> JoinEventAsync(string activityId, string userId)
    {
        var activity = await _EventRepository.GetByIdAsync(activityId);
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

        return await _EventRepository.JoinActivityAsync(activityId, userId);
    }

    public async Task<bool> LeaveEventAsync(string activityId, string userId)
    {
        var activity = await _EventRepository.GetByIdAsync(activityId);
        if (activity == null)
        {
            return false;
        }

        if (!activity.Participants.Contains(userId))
        {
            throw new InvalidOperationException("User is not part of this activity");
        }

        return await _EventRepository.LeaveActivityAsync(activityId, userId);
    }
}
