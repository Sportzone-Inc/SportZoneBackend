using SportZone.Models;

namespace SportZone.Repositories;

public interface IReviewRepository
{
    Task<Review> CreateAsync(Review review);
    Task<Review?> GetByIdAsync(string id);
    Task<IEnumerable<Review>> GetByActivityIdAsync(string activityId);
    Task<IEnumerable<Review>> GetByReviewerIdAsync(string reviewerId);
    Task<IEnumerable<Review>> GetByRevieweeIdAsync(string revieweeId);
    Task<double> GetAverageRatingForActivityAsync(string activityId);
    Task<double> GetAverageRatingForUserAsync(string userId);
    Task<bool> UpdateAsync(string id, Review review);
    Task<bool> AddResponseAsync(string id, string response);
    Task<bool> VoteHelpfulAsync(string id, string userId);
    Task<bool> DeleteAsync(string id);
}
