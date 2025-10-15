using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using SportZone.Services;

namespace SportZone.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly IPasswordHasher _passwordHasher;

    public UserRepository(IOptions<MongoDbSettings> mongoDbSettings, IPasswordHasher passwordHasher)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<User>(mongoDbSettings.Value.UsersCollectionName);
        _passwordHasher = passwordHasher;
    }

    public async Task<User> CreateAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await _usersCollection.InsertOneAsync(user);
        return user;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _usersCollection.Find(x => x.Username == username).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _usersCollection.Find(_ => true).ToListAsync();
    }

    public async Task<bool> UpdateAsync(string id, User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        var result = await _usersCollection.ReplaceOneAsync(x => x.Id == id, user);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _usersCollection.DeleteOneAsync(x => x.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<bool> VerifyPasswordAsync(string username, string password)
    {
        // Validate input parameters
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        // Get user from database
        var user = await GetByUsernameAsync(username);
        
        // Check if user exists
        if (user == null)
        {
            return false;
        }

        // Check if password hash exists
        if (string.IsNullOrWhiteSpace(user.Password))
        {
            return false;
        }

        // Verify password
        return _passwordHasher.VerifyPassword(password, user.Password);
    }
}
