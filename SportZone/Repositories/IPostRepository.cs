using SportZone.Models;

namespace SportZone.Repositories;

public interface IPostRepository
{
    Task<Post> CreateAsync(Post post);
    Task<Post?> GetByIdAsync(string id);
    Task<IEnumerable<Post>> GetAllAsync();
    Task<IEnumerable<Post>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Post>> GetByActivityIdAsync(string activityId);
    Task<IEnumerable<Post>> GetFeedForUserAsync(string userId, int skip = 0, int limit = 20);
    Task<bool> UpdateAsync(string id, Post post);
    Task<bool> DeleteAsync(string id);
    Task<bool> LikeAsync(string postId, string userId);
    Task<bool> UnlikeAsync(string postId, string userId);
    Task<bool> IncrementCommentsCountAsync(string postId);
    Task<bool> DecrementCommentsCountAsync(string postId);
}
