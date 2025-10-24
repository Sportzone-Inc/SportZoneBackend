using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using Microsoft.Extensions.Options;

namespace SportZone.Repositories;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly IMongoCollection<UserSettings> _settings;

    public UserSettingsRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _settings = database.GetCollection<UserSettings>("user_settings");

        // Create unique index on userId
        var indexOptions = new CreateIndexOptions { Unique = true };
        _settings.Indexes.CreateOneAsync(
            new CreateIndexModel<UserSettings>(Builders<UserSettings>.IndexKeys.Ascending(s => s.UserId), indexOptions));
    }

    public async Task<UserSettings> CreateAsync(UserSettings settings)
    {
        await _settings.InsertOneAsync(settings);
        return settings;
    }

    public async Task<UserSettings?> GetByIdAsync(string id)
    {
        return await _settings.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<UserSettings?> GetByUserIdAsync(string userId)
    {
        return await _settings.Find(s => s.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateAsync(string id, UserSettings settings)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        var result = await _settings.ReplaceOneAsync(s => s.Id == id, settings);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _settings.DeleteOneAsync(s => s.Id == id);
        return result.DeletedCount > 0;
    }
}
