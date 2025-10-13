using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;

namespace SportZone.Repositories;

public class SportActivityRepository : ISportActivityRepository
{
    private readonly IMongoCollection<SportActivity> _sportActivitiesCollection;

    public SportActivityRepository(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _sportActivitiesCollection = mongoDatabase.GetCollection<SportActivity>("SportActivities");
    }

    public async Task<SportActivity> CreateAsync(SportActivity sportActivity)
    {
        sportActivity.UniqueId = Guid.NewGuid().ToString();
        sportActivity.CreatedAt = DateTime.UtcNow;
        sportActivity.UpdatedAt = DateTime.UtcNow;
        await _sportActivitiesCollection.InsertOneAsync(sportActivity);
        return sportActivity;
    }

    public async Task<SportActivity?> GetByIdAsync(string id)
    {
        return await _sportActivitiesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<SportActivity?> GetByUniqueIdAsync(string uniqueId)
    {
        return await _sportActivitiesCollection.Find(x => x.UniqueId == uniqueId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SportActivity>> GetAllAsync()
    {
        return await _sportActivitiesCollection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<SportActivity>> GetByUserIdAsync(string userId)
    {
        return await _sportActivitiesCollection.Find(x => x.CreatedBy == userId).ToListAsync();
    }

    public async Task<IEnumerable<SportActivity>> GetBySportTypeAsync(SportType sportType)
    {
        return await _sportActivitiesCollection.Find(x => x.SportType == sportType).ToListAsync();
    }

    public async Task<IEnumerable<SportActivity>> GetActiveActivitiesAsync()
    {
        return await _sportActivitiesCollection.Find(x => x.IsActive == true).ToListAsync();
    }

    public async Task<bool> UpdateAsync(string id, SportActivity sportActivity)
    {
        sportActivity.UpdatedAt = DateTime.UtcNow;
        var result = await _sportActivitiesCollection.ReplaceOneAsync(x => x.Id == id, sportActivity);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _sportActivitiesCollection.DeleteOneAsync(x => x.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<bool> JoinActivityAsync(string activityId, string userId)
    {
        var filter = Builders<SportActivity>.Filter.Eq(x => x.Id, activityId);
        var update = Builders<SportActivity>.Update
            .AddToSet(x => x.Participants, userId)
            .Inc(x => x.CurrentParticipants, 1)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _sportActivitiesCollection.UpdateOneAsync(filter, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> LeaveActivityAsync(string activityId, string userId)
    {
        var filter = Builders<SportActivity>.Filter.Eq(x => x.Id, activityId);
        var update = Builders<SportActivity>.Update
            .Pull(x => x.Participants, userId)
            .Inc(x => x.CurrentParticipants, -1)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _sportActivitiesCollection.UpdateOneAsync(filter, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
