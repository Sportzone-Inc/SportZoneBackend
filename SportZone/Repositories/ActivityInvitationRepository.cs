using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using Microsoft.Extensions.Options;

namespace SportZone.Repositories;

public class ActivityInvitationRepository : IActivityInvitationRepository
{
    private readonly IMongoCollection<ActivityInvitation> _invitations;

    public ActivityInvitationRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _invitations = database.GetCollection<ActivityInvitation>("activity_invitations");

        // Create indexes
        var compoundIndexKeys = Builders<ActivityInvitation>.IndexKeys
            .Ascending(i => i.ActivityId)
            .Ascending(i => i.ReceiverId);
        var compoundIndexOptions = new CreateIndexOptions { Unique = true };
        _invitations.Indexes.CreateOneAsync(
            new CreateIndexModel<ActivityInvitation>(compoundIndexKeys, compoundIndexOptions));

        _invitations.Indexes.CreateOneAsync(
            new CreateIndexModel<ActivityInvitation>(Builders<ActivityInvitation>.IndexKeys.Ascending(i => i.ReceiverId)));
        _invitations.Indexes.CreateOneAsync(
            new CreateIndexModel<ActivityInvitation>(Builders<ActivityInvitation>.IndexKeys.Ascending(i => i.Status)));
    }

    public async Task<ActivityInvitation> CreateAsync(ActivityInvitation invitation)
    {
        await _invitations.InsertOneAsync(invitation);
        return invitation;
    }

    public async Task<ActivityInvitation?> GetByIdAsync(string id)
    {
        return await _invitations.Find(i => i.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ActivityInvitation>> GetByReceiverIdAsync(string receiverId)
    {
        return await _invitations.Find(i => i.ReceiverId == receiverId)
            .SortByDescending(i => i.SentAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActivityInvitation>> GetByActivityIdAsync(string activityId)
    {
        return await _invitations.Find(i => i.ActivityId == activityId).ToListAsync();
    }

    public async Task<IEnumerable<ActivityInvitation>> GetByStatusAsync(string receiverId, InvitationStatus status)
    {
        return await _invitations.Find(i => i.ReceiverId == receiverId && i.Status == status)
            .SortByDescending(i => i.SentAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(string id, InvitationStatus status)
    {
        var update = Builders<ActivityInvitation>.Update
            .Set(i => i.Status, status)
            .Set(i => i.RespondedAt, DateTime.UtcNow);

        var result = await _invitations.UpdateOneAsync(i => i.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _invitations.DeleteOneAsync(i => i.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> ExpireOldInvitationsAsync()
    {
        var filter = Builders<ActivityInvitation>.Filter.And(
            Builders<ActivityInvitation>.Filter.Eq(i => i.Status, InvitationStatus.Pending),
            Builders<ActivityInvitation>.Filter.Lt(i => i.ExpiresAt, DateTime.UtcNow)
        );

        var update = Builders<ActivityInvitation>.Update.Set(i => i.Status, InvitationStatus.Expired);
        var result = await _invitations.UpdateManyAsync(filter, update);
        return result.ModifiedCount > 0;
    }
}
