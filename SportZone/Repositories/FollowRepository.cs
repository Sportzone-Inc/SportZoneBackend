using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using Microsoft.Extensions.Options;

namespace SportZone.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly IMongoCollection<Follow> _follows;

    public FollowRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _follows = database.GetCollection<Follow>("follows");

        // Create unique compound index on followerId and followingId
        var indexKeys = Builders<Follow>.IndexKeys
            .Ascending(f => f.FollowerId)
            .Ascending(f => f.FollowingId);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<Follow>(indexKeys, indexOptions);
        _follows.Indexes.CreateOneAsync(indexModel);

        // Create individual indexes
        _follows.Indexes.CreateOneAsync(
            new CreateIndexModel<Follow>(Builders<Follow>.IndexKeys.Ascending(f => f.FollowerId)));
        _follows.Indexes.CreateOneAsync(
            new CreateIndexModel<Follow>(Builders<Follow>.IndexKeys.Ascending(f => f.FollowingId)));
    }

    public async Task<Follow> CreateAsync(Follow follow)
    {
        await _follows.InsertOneAsync(follow);
        return follow;
    }

    public async Task<Follow?> GetByIdAsync(string id)
    {
        return await _follows.Find(f => f.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Follow?> GetByFollowerAndFollowingAsync(string followerId, string followingId)
    {
        return await _follows.Find(f => f.FollowerId == followerId && f.FollowingId == followingId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Follow>> GetFollowersByUserIdAsync(string userId)
    {
        return await _follows.Find(f => f.FollowingId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Follow>> GetFollowingByUserIdAsync(string userId)
    {
        return await _follows.Find(f => f.FollowerId == userId).ToListAsync();
    }

    public async Task<int> GetFollowersCountAsync(string userId)
    {
        return (int)await _follows.CountDocumentsAsync(f => f.FollowingId == userId);
    }

    public async Task<int> GetFollowingCountAsync(string userId)
    {
        return (int)await _follows.CountDocumentsAsync(f => f.FollowerId == userId);
    }

    public async Task<bool> IsFollowingAsync(string followerId, string followingId)
    {
        var count = await _follows.CountDocumentsAsync(
            f => f.FollowerId == followerId && f.FollowingId == followingId);
        return count > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _follows.DeleteOneAsync(f => f.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteByFollowerAndFollowingAsync(string followerId, string followingId)
    {
        var result = await _follows.DeleteOneAsync(
            f => f.FollowerId == followerId && f.FollowingId == followingId);
        return result.DeletedCount > 0;
    }
}
