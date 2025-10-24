using SportZone.Models;

namespace SportZone.Repositories;

public interface INotificationRepository
{
    Task<Notification> CreateAsync(Notification notification);
    Task<Notification?> GetByIdAsync(string id);
    Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, bool? isRead = null, int skip = 0, int limit = 50);
    Task<int> GetUnreadCountAsync(string userId);
    Task<bool> MarkAsReadAsync(string id);
    Task<bool> MarkAllAsReadAsync(string userId);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteAllForUserAsync(string userId);
}
