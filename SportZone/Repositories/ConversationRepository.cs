using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using Microsoft.Extensions.Options;

namespace SportZone.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly IMongoCollection<Conversation> _conversations;

    public ConversationRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _conversations = database.GetCollection<Conversation>("conversations");

        // Create indexes
        _conversations.Indexes.CreateOneAsync(
            new CreateIndexModel<Conversation>(Builders<Conversation>.IndexKeys.Ascending(c => c.Participants)));
        _conversations.Indexes.CreateOneAsync(
            new CreateIndexModel<Conversation>(Builders<Conversation>.IndexKeys.Descending(c => c.LastMessageAt)));
    }

    public async Task<Conversation> CreateAsync(Conversation conversation)
    {
        await _conversations.InsertOneAsync(conversation);
        return conversation;
    }

    public async Task<Conversation?> GetByIdAsync(string id)
    {
        return await _conversations.Find(c => c.Id == id && c.IsActive).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Conversation>> GetByUserIdAsync(string userId)
    {
        var filter = Builders<Conversation>.Filter.And(
            Builders<Conversation>.Filter.AnyEq(c => c.Participants, userId),
            Builders<Conversation>.Filter.Eq(c => c.IsActive, true)
        );

        return await _conversations.Find(filter)
            .SortByDescending(c => c.LastMessageAt)
            .ToListAsync();
    }

    public async Task<Conversation?> GetDirectConversationAsync(string userId1, string userId2)
    {
        var filter = Builders<Conversation>.Filter.And(
            Builders<Conversation>.Filter.Eq(c => c.ConversationType, ConversationType.Direct),
            Builders<Conversation>.Filter.All(c => c.Participants, new[] { userId1, userId2 }),
            Builders<Conversation>.Filter.Size(c => c.Participants, 2),
            Builders<Conversation>.Filter.Eq(c => c.IsActive, true)
        );

        return await _conversations.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateAsync(string id, Conversation conversation)
    {
        conversation.UpdatedAt = DateTime.UtcNow;
        var result = await _conversations.ReplaceOneAsync(c => c.Id == id, conversation);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateLastMessageAsync(string id, string messageId, DateTime timestamp)
    {
        var update = Builders<Conversation>.Update
            .Set(c => c.LastMessageId, messageId)
            .Set(c => c.LastMessageAt, timestamp)
            .Set(c => c.UpdatedAt, DateTime.UtcNow);

        var result = await _conversations.UpdateOneAsync(c => c.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var update = Builders<Conversation>.Update.Set(c => c.IsActive, false);
        var result = await _conversations.UpdateOneAsync(c => c.Id == id, update);
        return result.ModifiedCount > 0;
    }
}
