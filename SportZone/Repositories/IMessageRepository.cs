using SportZone.Models;

namespace SportZone.Repositories;

public interface IMessageRepository
{
    Task<Message> CreateAsync(Message message);
    Task<Message?> GetByIdAsync(string id);
    Task<IEnumerable<Message>> GetByConversationIdAsync(string conversationId, int skip = 0, int limit = 50);
    Task<int> GetUnreadCountAsync(string conversationId, string userId);
    Task<bool> UpdateAsync(string id, Message message);
    Task<bool> MarkAsReadAsync(string messageId, string userId);
    Task<bool> MarkConversationAsReadAsync(string conversationId, string userId);
    Task<bool> DeleteAsync(string id);
}
