using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using Microsoft.Extensions.Options;

namespace SportZone.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _notifications;

    public NotificationRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _notifications = database.GetCollection<Notification>("notifications");

        // Create indexes
        _notifications.Indexes.CreateOneAsync(
            new CreateIndexModel<Notification>(Builders<Notification>.IndexKeys.Ascending(n => n.UserId)));
        _notifications.Indexes.CreateOneAsync(
            new CreateIndexModel<Notification>(Builders<Notification>.IndexKeys.Ascending(n => n.IsRead)));
        _notifications.Indexes.CreateOneAsync(
            new CreateIndexModel<Notification>(Builders<Notification>.IndexKeys.Descending(n => n.CreatedAt)));
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        await _notifications.InsertOneAsync(notification);
        return notification;
    }

    public async Task<Notification?> GetByIdAsync(string id)
    {
        return await _notifications.Find(n => n.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, bool? isRead = null, int skip = 0, int limit = 50)
    {
        var filterBuilder = Builders<Notification>.Filter;
        var filter = filterBuilder.Eq(n => n.UserId, userId);

        if (isRead.HasValue)
            filter = filterBuilder.And(filter, filterBuilder.Eq(n => n.IsRead, isRead.Value));

        return await _notifications.Find(filter)
            .SortByDescending(n => n.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        var filter = Builders<Notification>.Filter.And(
            Builders<Notification>.Filter.Eq(n => n.UserId, userId),
            Builders<Notification>.Filter.Eq(n => n.IsRead, false)
        );

        return (int)await _notifications.CountDocumentsAsync(filter);
    }

    public async Task<bool> MarkAsReadAsync(string id)
    {
        var update = Builders<Notification>.Update
            .Set(n => n.IsRead, true)
            .Set(n => n.ReadAt, DateTime.UtcNow);

        var result = await _notifications.UpdateOneAsync(n => n.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> MarkAllAsReadAsync(string userId)
    {
        var filter = Builders<Notification>.Filter.And(
            Builders<Notification>.Filter.Eq(n => n.UserId, userId),
            Builders<Notification>.Filter.Eq(n => n.IsRead, false)
        );

        var update = Builders<Notification>.Update
            .Set(n => n.IsRead, true)
            .Set(n => n.ReadAt, DateTime.UtcNow);

        var result = await _notifications.UpdateManyAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _notifications.DeleteOneAsync(n => n.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteAllForUserAsync(string userId)
    {
        var result = await _notifications.DeleteManyAsync(n => n.UserId == userId);
        return result.DeletedCount > 0;
    }
}
