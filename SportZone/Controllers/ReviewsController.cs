using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportZone.DTOs;
using SportZone.Models;
using SportZone.Repositories;
using System.Security.Claims;

namespace SportZone.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(
        IReviewRepository reviewRepository,
        ILogger<ReviewsController> logger)
    {
        _reviewRepository = reviewRepository;
        _logger = logger;
    }

    private string GetCurrentUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    /// <summary>
    /// Create a review
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ReviewResponseDto>> CreateReview([FromBody] CreateReviewDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var review = new Review
            {
                ReviewType = dto.ReviewType,
                ActivityId = dto.ActivityId,
                ReviewerId = userId,
                RevieweeId = dto.RevieweeId,
                Rating = dto.Rating,
                Title = dto.Title,
                Comment = dto.Comment
            };

            var created = await _reviewRepository.CreateAsync(review);
            var response = MapToResponseDto(created);

            return CreatedAtAction(nameof(GetReview), new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating review");
            return StatusCode(500, "An error occurred while creating the review");
        }
    }

    /// <summary>
    /// Get review by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponseDto>> GetReview(string id)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return NotFound();

            return Ok(MapToResponseDto(review));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting review");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get reviews for an activity
    /// </summary>
    [HttpGet("activity/{activityId}")]
    public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetActivityReviews(string activityId)
    {
        try
        {
            var reviews = await _reviewRepository.GetByActivityIdAsync(activityId);
            var response = reviews.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting activity reviews");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get average rating for an activity
    /// </summary>
    [HttpGet("activity/{activityId}/average-rating")]
    public async Task<ActionResult<double>> GetActivityAverageRating(string activityId)
    {
        try
        {
            var average = await _reviewRepository.GetAverageRatingForActivityAsync(activityId);
            return Ok(average);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting average rating");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get reviews by a reviewer
    /// </summary>
    [HttpGet("reviewer/{reviewerId}")]
    public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetReviewsByReviewer(string reviewerId)
    {
        try
        {
            var reviews = await _reviewRepository.GetByReviewerIdAsync(reviewerId);
            var response = reviews.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reviewer reviews");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get reviews for a user (as reviewee)
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetUserReviews(string userId)
    {
        try
        {
            var reviews = await _reviewRepository.GetByRevieweeIdAsync(userId);
            var response = reviews.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user reviews");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get average rating for a user
    /// </summary>
    [HttpGet("user/{userId}/average-rating")]
    public async Task<ActionResult<double>> GetUserAverageRating(string userId)
    {
        try
        {
            var average = await _reviewRepository.GetAverageRatingForUserAsync(userId);
            return Ok(average);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user average rating");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Update a review
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(string id, [FromBody] UpdateReviewDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return NotFound();

            if (review.ReviewerId != userId)
                return Forbid("You can only update your own reviews");

            if (dto.Rating.HasValue) review.Rating = dto.Rating.Value;
            if (dto.Title != null) review.Title = dto.Title;
            if (dto.Comment != null) review.Comment = dto.Comment;

            var updated = await _reviewRepository.UpdateAsync(id, review);
            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating review");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Add a response to a review (for organizers)
    /// </summary>
    [HttpPost("{id}/response")]
    public async Task<IActionResult> AddResponse(string id, [FromBody] AddReviewResponseDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var responded = await _reviewRepository.AddResponseAsync(id, dto.Response);
            if (!responded)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding response");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Vote a review as helpful
    /// </summary>
    [HttpPost("{id}/helpful")]
    public async Task<IActionResult> VoteHelpful(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var voted = await _reviewRepository.VoteHelpfulAsync(id, userId);
            if (!voted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error voting helpful");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Delete a review
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return NotFound();

            if (review.ReviewerId != userId)
                return Forbid("You can only delete your own reviews");

            var deleted = await _reviewRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting review");
            return StatusCode(500, "An error occurred");
        }
    }

    private static ReviewResponseDto MapToResponseDto(Review r) => new()
    {
        Id = r.Id!,
        ReviewType = r.ReviewType,
        ActivityId = r.ActivityId,
        ReviewerId = r.ReviewerId,
        RevieweeId = r.RevieweeId,
        Rating = r.Rating,
        Title = r.Title,
        Comment = r.Comment,
        HelpfulCount = r.HelpfulCount,
        IsVerified = r.IsVerified,
        Response = r.Response,
        RespondedAt = r.RespondedAt,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}
