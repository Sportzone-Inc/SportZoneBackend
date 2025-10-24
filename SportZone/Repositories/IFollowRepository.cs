using SportZone.Models;

namespace SportZone.Repositories;

public interface IFollowRepository
{
    Task<Follow> CreateAsync(Follow follow);
    Task<Follow?> GetByIdAsync(string id);
    Task<Follow?> GetByFollowerAndFollowingAsync(string followerId, string followingId);
    Task<IEnumerable<Follow>> GetFollowersByUserIdAsync(string userId);
    Task<IEnumerable<Follow>> GetFollowingByUserIdAsync(string userId);
    Task<int> GetFollowersCountAsync(string userId);
    Task<int> GetFollowingCountAsync(string userId);
    Task<bool> IsFollowingAsync(string followerId, string followingId);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteByFollowerAndFollowingAsync(string followerId, string followingId);
}
