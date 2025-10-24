using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using Microsoft.Extensions.Options;

namespace SportZone.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly IMongoCollection<Message> _messages;

    public MessageRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _messages = database.GetCollection<Message>("messages");

        // Create indexes
        _messages.Indexes.CreateOneAsync(
            new CreateIndexModel<Message>(Builders<Message>.IndexKeys.Ascending(m => m.ConversationId)));
        _messages.Indexes.CreateOneAsync(
            new CreateIndexModel<Message>(Builders<Message>.IndexKeys.Ascending(m => m.SenderId)));
        _messages.Indexes.CreateOneAsync(
            new CreateIndexModel<Message>(Builders<Message>.IndexKeys.Descending(m => m.CreatedAt)));
    }

    public async Task<Message> CreateAsync(Message message)
    {
        await _messages.InsertOneAsync(message);
        return message;
    }

    public async Task<Message?> GetByIdAsync(string id)
    {
        return await _messages.Find(m => m.Id == id && !m.IsDeleted).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Message>> GetByConversationIdAsync(string conversationId, int skip = 0, int limit = 50)
    {
        return await _messages.Find(m => m.ConversationId == conversationId && !m.IsDeleted)
            .SortByDescending(m => m.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string conversationId, string userId)
    {
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.ConversationId, conversationId),
            Builders<Message>.Filter.Ne(m => m.SenderId, userId),
            Builders<Message>.Filter.Not(
                Builders<Message>.Filter.ElemMatch(m => m.ReadBy, Builders<ReadReceipt>.Filter.Eq(r => r.UserId, userId))
            ),
            Builders<Message>.Filter.Eq(m => m.IsDeleted, false)
        );

        return (int)await _messages.CountDocumentsAsync(filter);
    }

    public async Task<bool> UpdateAsync(string id, Message message)
    {
        message.UpdatedAt = DateTime.UtcNow;
        message.IsEdited = true;
        var result = await _messages.ReplaceOneAsync(m => m.Id == id, message);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> MarkAsReadAsync(string messageId, string userId)
    {
        var readReceipt = new ReadReceipt { UserId = userId, ReadAt = DateTime.UtcNow };
        var update = Builders<Message>.Update.AddToSet(m => m.ReadBy, readReceipt);
        var result = await _messages.UpdateOneAsync(m => m.Id == messageId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> MarkConversationAsReadAsync(string conversationId, string userId)
    {
        var readReceipt = new ReadReceipt { UserId = userId, ReadAt = DateTime.UtcNow };
        
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.ConversationId, conversationId),
            Builders<Message>.Filter.Ne(m => m.SenderId, userId),
            Builders<Message>.Filter.Not(
                Builders<Message>.Filter.ElemMatch(m => m.ReadBy, Builders<ReadReceipt>.Filter.Eq(r => r.UserId, userId))
            )
        );

        var update = Builders<Message>.Update.AddToSet(m => m.ReadBy, readReceipt);
        var result = await _messages.UpdateManyAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var update = Builders<Message>.Update.Set(m => m.IsDeleted, true);
        var result = await _messages.UpdateOneAsync(m => m.Id == id, update);
        return result.ModifiedCount > 0;
    }
}
