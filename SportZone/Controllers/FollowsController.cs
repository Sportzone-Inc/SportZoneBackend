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
public class FollowsController : ControllerBase
{
    private readonly IFollowRepository _followRepository;
    private readonly ILogger<FollowsController> _logger;

    public FollowsController(IFollowRepository followRepository, ILogger<FollowsController> logger)
    {
        _followRepository = followRepository;
        _logger = logger;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Follow a user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FollowResponseDto>> FollowUser([FromBody] CreateFollowDto dto)
    {
        try
        {
            var followerId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(followerId))
                return Unauthorized("User not authenticated");

            if (followerId == dto.FollowingId)
                return BadRequest("Cannot follow yourself");

            // Check if already following
            var existingFollow = await _followRepository.GetByFollowerAndFollowingAsync(followerId, dto.FollowingId);
            if (existingFollow != null)
                return BadRequest("Already following this user");

            var follow = new Follow
            {
                FollowerId = followerId,
                FollowingId = dto.FollowingId
            };

            var created = await _followRepository.CreateAsync(follow);

            var response = new FollowResponseDto
            {
                Id = created.Id!,
                FollowerId = created.FollowerId,
                FollowingId = created.FollowingId,
                CreatedAt = created.CreatedAt
            };

            return CreatedAtAction(nameof(GetFollow), new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating follow");
            return StatusCode(500, "An error occurred while following the user");
        }
    }

    /// <summary>
    /// Unfollow a user
    /// </summary>
    [HttpDelete("{followingId}")]
    public async Task<IActionResult> UnfollowUser(string followingId)
    {
        try
        {
            var followerId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(followerId))
                return Unauthorized("User not authenticated");

            var deleted = await _followRepository.DeleteByFollowerAndFollowingAsync(followerId, followingId);
            
            if (!deleted)
                return NotFound("Follow relationship not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfollowing user");
            return StatusCode(500, "An error occurred while unfollowing the user");
        }
    }

    /// <summary>
    /// Get a specific follow relationship
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<FollowResponseDto>> GetFollow(string id)
    {
        try
        {
            var follow = await _followRepository.GetByIdAsync(id);
            
            if (follow == null)
                return NotFound();

            var response = new FollowResponseDto
            {
                Id = follow.Id!,
                FollowerId = follow.FollowerId,
                FollowingId = follow.FollowingId,
                CreatedAt = follow.CreatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting follow");
            return StatusCode(500, "An error occurred while retrieving the follow");
        }
    }

    /// <summary>
    /// Get followers of a user
    /// </summary>
    [HttpGet("user/{userId}/followers")]
    public async Task<ActionResult<IEnumerable<FollowResponseDto>>> GetFollowers(string userId)
    {
        try
        {
            var follows = await _followRepository.GetFollowersByUserIdAsync(userId);
            
            var response = follows.Select(f => new FollowResponseDto
            {
                Id = f.Id!,
                FollowerId = f.FollowerId,
                FollowingId = f.FollowingId,
                CreatedAt = f.CreatedAt
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followers");
            return StatusCode(500, "An error occurred while retrieving followers");
        }
    }

    /// <summary>
    /// Get users that a user is following
    /// </summary>
    [HttpGet("user/{userId}/following")]
    public async Task<ActionResult<IEnumerable<FollowResponseDto>>> GetFollowing(string userId)
    {
        try
        {
            var follows = await _followRepository.GetFollowingByUserIdAsync(userId);
            
            var response = follows.Select(f => new FollowResponseDto
            {
                Id = f.Id!,
                FollowerId = f.FollowerId,
                FollowingId = f.FollowingId,
                CreatedAt = f.CreatedAt
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following");
            return StatusCode(500, "An error occurred while retrieving following");
        }
    }

    /// <summary>
    /// Get follow statistics for a user
    /// </summary>
    [HttpGet("user/{userId}/stats")]
    public async Task<ActionResult<FollowStatsDto>> GetFollowStats(string userId)
    {
        try
        {
            var followersCount = await _followRepository.GetFollowersCountAsync(userId);
            var followingCount = await _followRepository.GetFollowingCountAsync(userId);

            var stats = new FollowStatsDto
            {
                FollowersCount = followersCount,
                FollowingCount = followingCount
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting follow stats");
            return StatusCode(500, "An error occurred while retrieving follow stats");
        }
    }

    /// <summary>
    /// Check if current user is following another user
    /// </summary>
    [HttpGet("is-following/{userId}")]
    public async Task<ActionResult<bool>> IsFollowing(string userId)
    {
        try
        {
            var followerId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(followerId))
                return Unauthorized("User not authenticated");

            var isFollowing = await _followRepository.IsFollowingAsync(followerId, userId);
            return Ok(isFollowing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking follow status");
            return StatusCode(500, "An error occurred while checking follow status");
        }
    }
}
