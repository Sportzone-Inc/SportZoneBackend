using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;

namespace SportZone.Repositories;

public class SportActivityRepository : ISportActivityRepository
{
    private readonly IMongoCollection<SportActivity> _sportActivities;

    public SportActivityRepository(IMongoDatabase database)
    {
        _sportActivities = database.GetCollection<SportActivity>(MongoDbSettings.SportActivitiesCollectionName);
    }

    public async Task<SportActivity> CreateAsync(SportActivity sportActivity)
    {
        await _sportActivities.InsertOneAsync(sportActivity);
        return sportActivity;
    }

    public async Task<SportActivity?> GetByIdAsync(string id)
    {
        return await _sportActivities.Find(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<SportActivity?> GetByUniqueIdAsync(string uniqueId)
    {
        return await _sportActivities.Find(a => a.UniqueId == uniqueId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SportActivity>> GetAllAsync()
    {
        return await _sportActivities.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<SportActivity>> GetByUserIdAsync(string userId)
    {
        return await _sportActivities.Find(a => a.CreatedBy == userId).ToListAsync();
    }

    public async Task<IEnumerable<SportActivity>> GetBySportTypeAsync(SportType sportType)
    {
        return await _sportActivities.Find(a => a.SportType == sportType).ToListAsync();
    }

    public async Task<IEnumerable<SportActivity>> GetActiveActivitiesAsync()
    {
        return await _sportActivities.Find(a => a.IsActive).ToListAsync();
    }

    public async Task<bool> UpdateAsync(string id, SportActivity sportActivity)
    {
        sportActivity.UpdatedAt = DateTime.UtcNow;
        var result = await _sportActivities.ReplaceOneAsync(a => a.Id == id, sportActivity);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _sportActivities.DeleteOneAsync(a => a.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> JoinActivityAsync(string activityId, string userId)
    {
        var update = Builders<SportActivity>.Update
            .AddToSet(a => a.Participants, userId)
            .Inc(a => a.CurrentParticipants, 1)
            .Set(a => a.UpdatedAt, DateTime.UtcNow);

        var result = await _sportActivities.UpdateOneAsync(a => a.Id == activityId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> LeaveActivityAsync(string activityId, string userId)
    {
        var update = Builders<SportActivity>.Update
            .Pull(a => a.Participants, userId)
            .Inc(a => a.CurrentParticipants, -1)
            .Set(a => a.UpdatedAt, DateTime.UtcNow);

        var result = await _sportActivities.UpdateOneAsync(a => a.Id == activityId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<IEnumerable<SportActivity>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var filter = Builders<SportActivity>.Filter.And(
            Builders<SportActivity>.Filter.Gte(a => a.ScheduledDate, startDate),
            Builders<SportActivity>.Filter.Lte(a => a.ScheduledDate, endDate),
            Builders<SportActivity>.Filter.Eq(a => a.IsActive, true)
        );

        return await _sportActivities.Find(filter).ToListAsync();
    }
}
