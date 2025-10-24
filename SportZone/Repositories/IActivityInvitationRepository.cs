using SportZone.Models;

namespace SportZone.Repositories;

public interface IActivityInvitationRepository
{
    Task<ActivityInvitation> CreateAsync(ActivityInvitation invitation);
    Task<ActivityInvitation?> GetByIdAsync(string id);
    Task<IEnumerable<ActivityInvitation>> GetByReceiverIdAsync(string receiverId);
    Task<IEnumerable<ActivityInvitation>> GetByActivityIdAsync(string activityId);
    Task<IEnumerable<ActivityInvitation>> GetByStatusAsync(string receiverId, InvitationStatus status);
    Task<bool> UpdateStatusAsync(string id, InvitationStatus status);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExpireOldInvitationsAsync();
}
