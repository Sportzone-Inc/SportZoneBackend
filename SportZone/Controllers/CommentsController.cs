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
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        ILogger<CommentsController> logger)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _logger = logger;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Create a new comment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CommentResponseDto>> CreateComment([FromBody] CreateCommentDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var comment = new Comment
            {
                PostId = dto.PostId,
                UserId = userId,
                ParentCommentId = dto.ParentCommentId,
                Body = dto.Body
            };

            var created = await _commentRepository.CreateAsync(comment);

            // Increment post comments count
            await _postRepository.IncrementCommentsCountAsync(dto.PostId);

            var response = MapToResponseDto(created);

            return CreatedAtAction(nameof(GetComment), new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comment");
            return StatusCode(500, "An error occurred while creating the comment");
        }
    }

    /// <summary>
    /// Get a comment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CommentResponseDto>> GetComment(string id)
    {
        try
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            
            if (comment == null)
                return NotFound();

            var response = MapToResponseDto(comment);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comment");
            return StatusCode(500, "An error occurred while retrieving the comment");
        }
    }

    /// <summary>
    /// Get comments by post ID
    /// </summary>
    [HttpGet("post/{postId}")]
    public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetCommentsByPost(string postId)
    {
        try
        {
            var comments = await _commentRepository.GetByPostIdAsync(postId);
            var response = comments.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments");
            return StatusCode(500, "An error occurred while retrieving comments");
        }
    }

    /// <summary>
    /// Get replies to a comment
    /// </summary>
    [HttpGet("{commentId}/replies")]
    public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetReplies(string commentId)
    {
        try
        {
            var replies = await _commentRepository.GetRepliesByCommentIdAsync(commentId);
            var response = replies.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting replies");
            return StatusCode(500, "An error occurred while retrieving replies");
        }
    }

    /// <summary>
    /// Update a comment
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(string id, [FromBody] UpdateCommentDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var comment = await _commentRepository.GetByIdAsync(id);
            
            if (comment == null)
                return NotFound();

            if (comment.UserId != userId)
                return Forbid("You can only update your own comments");

            comment.Body = dto.Body;

            var updated = await _commentRepository.UpdateAsync(id, comment);
            
            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment");
            return StatusCode(500, "An error occurred while updating the comment");
        }
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var comment = await _commentRepository.GetByIdAsync(id);
            
            if (comment == null)
                return NotFound();

            if (comment.UserId != userId)
                return Forbid("You can only delete your own comments");

            var deleted = await _commentRepository.DeleteAsync(id);
            
            if (!deleted)
                return NotFound();

            // Decrement post comments count
            await _postRepository.DecrementCommentsCountAsync(comment.PostId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment");
            return StatusCode(500, "An error occurred while deleting the comment");
        }
    }

    /// <summary>
    /// Like a comment
    /// </summary>
    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikeComment(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var liked = await _commentRepository.LikeAsync(id, userId);
            
            if (!liked)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error liking comment");
            return StatusCode(500, "An error occurred while liking the comment");
        }
    }

    /// <summary>
    /// Unlike a comment
    /// </summary>
    [HttpDelete("{id}/like")]
    public async Task<IActionResult> UnlikeComment(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var unliked = await _commentRepository.UnlikeAsync(id, userId);
            
            if (!unliked)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unliking comment");
            return StatusCode(500, "An error occurred while unliking the comment");
        }
    }

    private static CommentResponseDto MapToResponseDto(Comment comment)
    {
        return new CommentResponseDto
        {
            Id = comment.Id!,
            PostId = comment.PostId,
            UserId = comment.UserId,
            ParentCommentId = comment.ParentCommentId,
            Body = comment.Body,
            LikesCount = comment.LikesCount,
            IsEdited = comment.IsEdited,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}
