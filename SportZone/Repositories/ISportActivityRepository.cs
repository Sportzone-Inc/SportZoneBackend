using SportZone.Models;

namespace SportZone.Repositories;

public interface IEventRepository
{
    Task<Event> CreateAsync(Event Event);
    Task<Event?> GetByIdAsync(string id);
    Task<Event?> GetByUniqueIdAsync(string uniqueId);
    Task<IEnumerable<Event>> GetAllAsync();
    Task<IEnumerable<Event>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Event>> GetBySportTypeAsync(SportType sportType);
    Task<IEnumerable<Event>> GetActiveActivitiesAsync();
    Task<bool> UpdateAsync(string id, Event Event);
    Task<bool> DeleteAsync(string id);
    Task<bool> JoinActivityAsync(string activityId, string userId);
    Task<bool> LeaveActivityAsync(string activityId, string userId);
}
