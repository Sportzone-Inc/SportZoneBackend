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
public class PostsController : ControllerBase
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IPostRepository postRepository, ILogger<PostsController> logger)
    {
        _postRepository = postRepository;
        _logger = logger;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Create a new post
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PostResponseDto>> CreatePost([FromBody] CreatePostDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var post = new Post
            {
                UserId = userId,
                ActivityId = dto.ActivityId,
                Title = dto.Title,
                Body = dto.Body,
                PostType = dto.PostType,
                MediaUrls = dto.MediaUrls ?? new List<string>(),
                ThumbnailUrl = dto.ThumbnailUrl,
                Visibility = dto.Visibility,
                Tags = dto.Tags ?? new List<string>(),
                Location = dto.Location,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude
            };

            var created = await _postRepository.CreateAsync(post);

            var response = MapToResponseDto(created);

            return CreatedAtAction(nameof(GetPost), new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating post");
            return StatusCode(500, "An error occurred while creating the post");
        }
    }

    /// <summary>
    /// Get a post by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PostResponseDto>> GetPost(string id)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(id);
            
            if (post == null)
                return NotFound();

            var response = MapToResponseDto(post);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting post");
            return StatusCode(500, "An error occurred while retrieving the post");
        }
    }

    /// <summary>
    /// Get all posts
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostResponseDto>>> GetAllPosts()
    {
        try
        {
            var posts = await _postRepository.GetAllAsync();
            var response = posts.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts");
            return StatusCode(500, "An error occurred while retrieving posts");
        }
    }

    /// <summary>
    /// Get posts by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<PostResponseDto>>> GetPostsByUser(string userId)
    {
        try
        {
            var posts = await _postRepository.GetByUserIdAsync(userId);
            var response = posts.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user posts");
            return StatusCode(500, "An error occurred while retrieving user posts");
        }
    }

    /// <summary>
    /// Get posts by activity ID
    /// </summary>
    [HttpGet("activity/{activityId}")]
    public async Task<ActionResult<IEnumerable<PostResponseDto>>> GetPostsByActivity(string activityId)
    {
        try
        {
            var posts = await _postRepository.GetByActivityIdAsync(activityId);
            var response = posts.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting activity posts");
            return StatusCode(500, "An error occurred while retrieving activity posts");
        }
    }

    /// <summary>
    /// Get feed for current user
    /// </summary>
    [HttpGet("feed")]
    public async Task<ActionResult<IEnumerable<PostResponseDto>>> GetFeed([FromQuery] int skip = 0, [FromQuery] int limit = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var posts = await _postRepository.GetFeedForUserAsync(userId, skip, limit);
            var response = posts.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting feed");
            return StatusCode(500, "An error occurred while retrieving the feed");
        }
    }

    /// <summary>
    /// Update a post
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(string id, [FromBody] UpdatePostDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var post = await _postRepository.GetByIdAsync(id);
            
            if (post == null)
                return NotFound();

            if (post.UserId != userId)
                return Forbid("You can only update your own posts");

            // Update fields
            if (dto.Title != null) post.Title = dto.Title;
            if (dto.Body != null) post.Body = dto.Body;
            if (dto.MediaUrls != null) post.MediaUrls = dto.MediaUrls;
            if (dto.ThumbnailUrl != null) post.ThumbnailUrl = dto.ThumbnailUrl;
            if (dto.Visibility.HasValue) post.Visibility = dto.Visibility.Value;
            if (dto.Tags != null) post.Tags = dto.Tags;
            if (dto.IsPinned.HasValue) post.IsPinned = dto.IsPinned.Value;

            var updated = await _postRepository.UpdateAsync(id, post);
            
            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post");
            return StatusCode(500, "An error occurred while updating the post");
        }
    }

    /// <summary>
    /// Delete a post
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var post = await _postRepository.GetByIdAsync(id);
            
            if (post == null)
                return NotFound();

            if (post.UserId != userId)
                return Forbid("You can only delete your own posts");

            var deleted = await _postRepository.DeleteAsync(id);
            
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post");
            return StatusCode(500, "An error occurred while deleting the post");
        }
    }

    /// <summary>
    /// Like a post
    /// </summary>
    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikePost(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var liked = await _postRepository.LikeAsync(id, userId);
            
            if (!liked)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error liking post");
            return StatusCode(500, "An error occurred while liking the post");
        }
    }

    /// <summary>
    /// Unlike a post
    /// </summary>
    [HttpDelete("{id}/like")]
    public async Task<IActionResult> UnlikePost(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var unliked = await _postRepository.UnlikeAsync(id, userId);
            
            if (!unliked)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unliking post");
            return StatusCode(500, "An error occurred while unliking the post");
        }
    }

    private static PostResponseDto MapToResponseDto(Post post)
    {
        return new PostResponseDto
        {
            Id = post.Id!,
            UserId = post.UserId,
            ActivityId = post.ActivityId,
            Title = post.Title,
            Body = post.Body,
            PostType = post.PostType,
            MediaUrls = post.MediaUrls,
            ThumbnailUrl = post.ThumbnailUrl,
            LikesCount = post.LikesCount,
            CommentsCount = post.CommentsCount,
            SharesCount = post.SharesCount,
            Visibility = post.Visibility,
            IsPinned = post.IsPinned,
            Tags = post.Tags,
            Location = post.Location,
            Latitude = post.Latitude,
            Longitude = post.Longitude,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}
