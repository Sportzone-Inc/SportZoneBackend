using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using Microsoft.Extensions.Options;

namespace SportZone.Repositories;

public class ActivityParticipantRepository : IActivityParticipantRepository
{
    private readonly IMongoCollection<ActivityParticipant> _participants;

    public ActivityParticipantRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _participants = database.GetCollection<ActivityParticipant>("activity_participants");

        // Create indexes
        var compoundIndexKeys = Builders<ActivityParticipant>.IndexKeys
            .Ascending(p => p.ActivityId)
            .Ascending(p => p.UserId);
        var compoundIndexOptions = new CreateIndexOptions { Unique = true };
        _participants.Indexes.CreateOneAsync(
            new CreateIndexModel<ActivityParticipant>(compoundIndexKeys, compoundIndexOptions));

        _participants.Indexes.CreateOneAsync(
            new CreateIndexModel<ActivityParticipant>(Builders<ActivityParticipant>.IndexKeys.Ascending(p => p.ActivityId)));
        _participants.Indexes.CreateOneAsync(
            new CreateIndexModel<ActivityParticipant>(Builders<ActivityParticipant>.IndexKeys.Ascending(p => p.UserId)));
        _participants.Indexes.CreateOneAsync(
            new CreateIndexModel<ActivityParticipant>(Builders<ActivityParticipant>.IndexKeys.Ascending(p => p.Status)));
    }

    public async Task<ActivityParticipant> CreateAsync(ActivityParticipant participant)
    {
        await _participants.InsertOneAsync(participant);
        return participant;
    }

    public async Task<ActivityParticipant?> GetByIdAsync(string id)
    {
        return await _participants.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<ActivityParticipant?> GetByActivityAndUserAsync(string activityId, string userId)
    {
        return await _participants.Find(p => p.ActivityId == activityId && p.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ActivityParticipant>> GetByActivityIdAsync(string activityId)
    {
        return await _participants.Find(p => p.ActivityId == activityId).ToListAsync();
    }

    public async Task<IEnumerable<ActivityParticipant>> GetByUserIdAsync(string userId)
    {
        return await _participants.Find(p => p.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<ActivityParticipant>> GetByStatusAsync(string activityId, ParticipantStatus status)
    {
        return await _participants.Find(p => p.ActivityId == activityId && p.Status == status).ToListAsync();
    }

    public async Task<int> GetParticipantCountAsync(string activityId, ParticipantStatus? status = null)
    {
        var filter = status.HasValue
            ? Builders<ActivityParticipant>.Filter.And(
                Builders<ActivityParticipant>.Filter.Eq(p => p.ActivityId, activityId),
                Builders<ActivityParticipant>.Filter.Eq(p => p.Status, status.Value))
            : Builders<ActivityParticipant>.Filter.Eq(p => p.ActivityId, activityId);

        return (int)await _participants.CountDocumentsAsync(filter);
    }

    public async Task<bool> UpdateAsync(string id, ActivityParticipant participant)
    {
        participant.UpdatedAt = DateTime.UtcNow;
        var result = await _participants.ReplaceOneAsync(p => p.Id == id, participant);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateStatusAsync(string id, ParticipantStatus status)
    {
        var update = Builders<ActivityParticipant>.Update
            .Set(p => p.Status, status)
            .Set(p => p.UpdatedAt, DateTime.UtcNow);
        
        if (status == ParticipantStatus.Cancelled)
            update = update.Set(p => p.CancelledAt, DateTime.UtcNow);
        if (status == ParticipantStatus.Attended)
            update = update.Set(p => p.AttendanceMarkedAt, DateTime.UtcNow);

        var result = await _participants.UpdateOneAsync(p => p.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _participants.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }
}
