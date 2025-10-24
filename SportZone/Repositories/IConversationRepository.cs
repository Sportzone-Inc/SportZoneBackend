using SportZone.Models;

namespace SportZone.Repositories;

public interface IConversationRepository
{
    Task<Conversation> CreateAsync(Conversation conversation);
    Task<Conversation?> GetByIdAsync(string id);
    Task<IEnumerable<Conversation>> GetByUserIdAsync(string userId);
    Task<Conversation?> GetDirectConversationAsync(string userId1, string userId2);
    Task<bool> UpdateAsync(string id, Conversation conversation);
    Task<bool> UpdateLastMessageAsync(string id, string messageId, DateTime timestamp);
    Task<bool> DeleteAsync(string id);
}
