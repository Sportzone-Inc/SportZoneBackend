using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using Microsoft.Extensions.Options;

namespace SportZone.Repositories;

public class PostRepository : IPostRepository
{
    private readonly IMongoCollection<Post> _posts;

    public PostRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _posts = database.GetCollection<Post>("posts");

        // Create indexes
        _posts.Indexes.CreateOneAsync(
            new CreateIndexModel<Post>(Builders<Post>.IndexKeys.Ascending(p => p.UserId)));
        _posts.Indexes.CreateOneAsync(
            new CreateIndexModel<Post>(Builders<Post>.IndexKeys.Ascending(p => p.ActivityId)));
        _posts.Indexes.CreateOneAsync(
            new CreateIndexModel<Post>(Builders<Post>.IndexKeys.Descending(p => p.CreatedAt)));
    }

    public async Task<Post> CreateAsync(Post post)
    {
        await _posts.InsertOneAsync(post);
        return post;
    }

    public async Task<Post?> GetByIdAsync(string id)
    {
        return await _posts.Find(p => p.Id == id && p.IsActive).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _posts.Find(p => p.IsActive)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(string userId)
    {
        return await _posts.Find(p => p.UserId == userId && p.IsActive)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByActivityIdAsync(string activityId)
    {
        return await _posts.Find(p => p.ActivityId == activityId && p.IsActive)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetFeedForUserAsync(string userId, int skip = 0, int limit = 20)
    {
        // This is a simplified feed - in production you'd get posts from followed users
        return await _posts.Find(p => p.IsActive && p.Visibility == PostVisibility.Public)
            .SortByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(string id, Post post)
    {
        post.UpdatedAt = DateTime.UtcNow;
        var result = await _posts.ReplaceOneAsync(p => p.Id == id, post);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        // Soft delete
        var update = Builders<Post>.Update.Set(p => p.IsActive, false);
        var result = await _posts.UpdateOneAsync(p => p.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> LikeAsync(string postId, string userId)
    {
        var update = Builders<Post>.Update
            .AddToSet(p => p.Likes, userId)
            .Inc(p => p.LikesCount, 1);
        var result = await _posts.UpdateOneAsync(p => p.Id == postId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UnlikeAsync(string postId, string userId)
    {
        var update = Builders<Post>.Update
            .Pull(p => p.Likes, userId)
            .Inc(p => p.LikesCount, -1);
        var result = await _posts.UpdateOneAsync(p => p.Id == postId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> IncrementCommentsCountAsync(string postId)
    {
        var update = Builders<Post>.Update.Inc(p => p.CommentsCount, 1);
        var result = await _posts.UpdateOneAsync(p => p.Id == postId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DecrementCommentsCountAsync(string postId)
    {
        var update = Builders<Post>.Update.Inc(p => p.CommentsCount, -1);
        var result = await _posts.UpdateOneAsync(p => p.Id == postId, update);
        return result.ModifiedCount > 0;
    }
}
