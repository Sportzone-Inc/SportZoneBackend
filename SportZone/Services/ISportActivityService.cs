using SportZone.Models;

namespace SportZone.Services;

public interface IEventService
{
    Task<Event> CreateEventAsync(Event Event);
    Task<Event?> GetEventByIdAsync(string id);
    Task<Event?> GetEventByUniqueIdAsync(string uniqueId);
    Task<IEnumerable<Event>> GetAlleventsAsync();
    Task<IEnumerable<Event>> GeteventsByUserAsync(string userId);
    Task<IEnumerable<Event>> GeteventsByTypeAsync(SportType sportType);
    Task<IEnumerable<Event>> GetActiveeventsAsync();
    Task<bool> UpdateEventAsync(string id, Event Event);
    Task<bool> DeleteEventAsync(string id);
    Task<bool> JoinEventAsync(string activityId, string userId);
    Task<bool> LeaveEventAsync(string activityId, string userId);
}
