using SportZone.Models;

namespace SportZone.Repositories;

public interface IActivityParticipantRepository
{
    Task<ActivityParticipant> CreateAsync(ActivityParticipant participant);
    Task<ActivityParticipant?> GetByIdAsync(string id);
    Task<ActivityParticipant?> GetByActivityAndUserAsync(string activityId, string userId);
    Task<IEnumerable<ActivityParticipant>> GetByActivityIdAsync(string activityId);
    Task<IEnumerable<ActivityParticipant>> GetByUserIdAsync(string userId);
    Task<IEnumerable<ActivityParticipant>> GetByStatusAsync(string activityId, ParticipantStatus status);
    Task<int> GetParticipantCountAsync(string activityId, ParticipantStatus? status = null);
    Task<bool> UpdateAsync(string id, ActivityParticipant participant);
    Task<bool> UpdateStatusAsync(string id, ParticipantStatus status);
    Task<bool> DeleteAsync(string id);
}
