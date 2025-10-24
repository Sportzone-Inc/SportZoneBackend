using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using Microsoft.Extensions.Options;

namespace SportZone.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly IMongoCollection<Review> _reviews;

    public ReviewRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _reviews = database.GetCollection<Review>("reviews");

        // Create indexes
        _reviews.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(Builders<Review>.IndexKeys.Ascending(r => r.ActivityId)));
        _reviews.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(Builders<Review>.IndexKeys.Ascending(r => r.ReviewerId)));
        _reviews.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(Builders<Review>.IndexKeys.Ascending(r => r.RevieweeId)));
        _reviews.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(Builders<Review>.IndexKeys.Descending(r => r.Rating)));
    }

    public async Task<Review> CreateAsync(Review review)
    {
        await _reviews.InsertOneAsync(review);
        return review;
    }

    public async Task<Review?> GetByIdAsync(string id)
    {
        return await _reviews.Find(r => r.Id == id && r.IsActive).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Review>> GetByActivityIdAsync(string activityId)
    {
        return await _reviews.Find(r => r.ActivityId == activityId && r.IsActive)
            .SortByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByReviewerIdAsync(string reviewerId)
    {
        return await _reviews.Find(r => r.ReviewerId == reviewerId && r.IsActive)
            .SortByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByRevieweeIdAsync(string revieweeId)
    {
        return await _reviews.Find(r => r.RevieweeId == revieweeId && r.IsActive)
            .SortByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<double> GetAverageRatingForActivityAsync(string activityId)
    {
        var reviews = await _reviews.Find(r => r.ActivityId == activityId && r.IsActive).ToListAsync();
        return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
    }

    public async Task<double> GetAverageRatingForUserAsync(string userId)
    {
        var reviews = await _reviews.Find(r => r.RevieweeId == userId && r.IsActive).ToListAsync();
        return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
    }

    public async Task<bool> UpdateAsync(string id, Review review)
    {
        review.UpdatedAt = DateTime.UtcNow;
        var result = await _reviews.ReplaceOneAsync(r => r.Id == id, review);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> AddResponseAsync(string id, string response)
    {
        var update = Builders<Review>.Update
            .Set(r => r.Response, response)
            .Set(r => r.RespondedAt, DateTime.UtcNow);

        var result = await _reviews.UpdateOneAsync(r => r.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> VoteHelpfulAsync(string id, string userId)
    {
        var update = Builders<Review>.Update
            .AddToSet(r => r.HelpfulVotes, userId)
            .Inc(r => r.HelpfulCount, 1);

        var result = await _reviews.UpdateOneAsync(r => r.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var update = Builders<Review>.Update.Set(r => r.IsActive, false);
        var result = await _reviews.UpdateOneAsync(r => r.Id == id, update);
        return result.ModifiedCount > 0;
    }
}
