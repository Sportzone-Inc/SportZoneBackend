using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using Microsoft.Extensions.Options;

namespace SportZone.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly IMongoCollection<Comment> _comments;

    public CommentRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _comments = database.GetCollection<Comment>("comments");

        // Create indexes
        _comments.Indexes.CreateOneAsync(
            new CreateIndexModel<Comment>(Builders<Comment>.IndexKeys.Ascending(c => c.PostId)));
        _comments.Indexes.CreateOneAsync(
            new CreateIndexModel<Comment>(Builders<Comment>.IndexKeys.Ascending(c => c.UserId)));
        _comments.Indexes.CreateOneAsync(
            new CreateIndexModel<Comment>(Builders<Comment>.IndexKeys.Ascending(c => c.ParentCommentId)));
    }

    public async Task<Comment> CreateAsync(Comment comment)
    {
        await _comments.InsertOneAsync(comment);
        return comment;
    }

    public async Task<Comment?> GetByIdAsync(string id)
    {
        return await _comments.Find(c => c.Id == id && c.IsActive).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Comment>> GetByPostIdAsync(string postId)
    {
        return await _comments.Find(c => c.PostId == postId && c.IsActive)
            .SortBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetRepliesByCommentIdAsync(string parentCommentId)
    {
        return await _comments.Find(c => c.ParentCommentId == parentCommentId && c.IsActive)
            .SortBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(string id, Comment comment)
    {
        comment.UpdatedAt = DateTime.UtcNow;
        comment.IsEdited = true;
        var result = await _comments.ReplaceOneAsync(c => c.Id == id, comment);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        // Soft delete
        var update = Builders<Comment>.Update.Set(c => c.IsActive, false);
        var result = await _comments.UpdateOneAsync(c => c.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> LikeAsync(string commentId, string userId)
    {
        var update = Builders<Comment>.Update
            .AddToSet(c => c.Likes, userId)
            .Inc(c => c.LikesCount, 1);
        var result = await _comments.UpdateOneAsync(c => c.Id == commentId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UnlikeAsync(string commentId, string userId)
    {
        var update = Builders<Comment>.Update
            .Pull(c => c.Likes, userId)
            .Inc(c => c.LikesCount, -1);
        var result = await _comments.UpdateOneAsync(c => c.Id == commentId, update);
        return result.ModifiedCount > 0;
    }
}
