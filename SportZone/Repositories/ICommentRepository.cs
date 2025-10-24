using SportZone.Models;

namespace SportZone.Repositories;

public interface ICommentRepository
{
    Task<Comment> CreateAsync(Comment comment);
    Task<Comment?> GetByIdAsync(string id);
    Task<IEnumerable<Comment>> GetByPostIdAsync(string postId);
    Task<IEnumerable<Comment>> GetRepliesByCommentIdAsync(string parentCommentId);
    Task<bool> UpdateAsync(string id, Comment comment);
    Task<bool> DeleteAsync(string id);
    Task<bool> LikeAsync(string commentId, string userId);
    Task<bool> UnlikeAsync(string commentId, string userId);
}
