using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;

namespace SportZone.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IMongoCollection<Event> _eventsCollection;

    public EventRepository(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _eventsCollection = mongoDatabase.GetCollection<Event>("events");
    }

    public async Task<Event> CreateAsync(Event Event)
    {
        Event.UniqueId = Guid.NewGuid().ToString();
        Event.CreatedAt = DateTime.UtcNow;
        Event.UpdatedAt = DateTime.UtcNow;
        await _eventsCollection.InsertOneAsync(Event);
        return Event;
    }

    public async Task<Event?> GetByIdAsync(string id)
    {
        return await _eventsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Event?> GetByUniqueIdAsync(string uniqueId)
    {
        return await _eventsCollection.Find(x => x.UniqueId == uniqueId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _eventsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetByUserIdAsync(string userId)
    {
        return await _eventsCollection.Find(x => x.CreatedBy == userId).ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetBySportTypeAsync(SportType sportType)
    {
        return await _eventsCollection.Find(x => x.SportType == sportType).ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetActiveActivitiesAsync()
    {
        return await _eventsCollection.Find(x => x.IsActive == true).ToListAsync();
    }

    public async Task<bool> UpdateAsync(string id, Event Event)
    {
        Event.UpdatedAt = DateTime.UtcNow;
        var result = await _eventsCollection.ReplaceOneAsync(x => x.Id == id, Event);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _eventsCollection.DeleteOneAsync(x => x.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<bool> JoinActivityAsync(string activityId, string userId)
    {
        var filter = Builders<Event>.Filter.Eq(x => x.Id, activityId);
        var update = Builders<Event>.Update
            .AddToSet(x => x.Participants, userId)
            .Inc(x => x.CurrentParticipants, 1)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _eventsCollection.UpdateOneAsync(filter, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> LeaveActivityAsync(string activityId, string userId)
    {
        var filter = Builders<Event>.Filter.Eq(x => x.Id, activityId);
        var update = Builders<Event>.Update
            .Pull(x => x.Participants, userId)
            .Inc(x => x.CurrentParticipants, -1)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _eventsCollection.UpdateOneAsync(filter, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
