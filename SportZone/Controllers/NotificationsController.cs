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
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationRepository notificationRepository,
        ILogger<NotificationsController> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    private string GetCurrentUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    /// <summary>
    /// Create a notification (typically called by system/services)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<NotificationResponseDto>> CreateNotification([FromBody] CreateNotificationDto dto)
    {
        try
        {
            var notification = new Notification
            {
                UserId = dto.UserId,
                NotificationType = dto.NotificationType,
                Title = dto.Title,
                Body = dto.Body,
                SenderId = dto.SenderId,
                ActivityId = dto.ActivityId,
                PostId = dto.PostId,
                CommentId = dto.CommentId,
                ActionUrl = dto.ActionUrl,
                DeepLink = dto.DeepLink
            };

            var created = await _notificationRepository.CreateAsync(notification);
            var response = MapToResponseDto(created);

            return CreatedAtAction(nameof(GetNotification), new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification");
            return StatusCode(500, "An error occurred while creating the notification");
        }
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationResponseDto>> GetNotification(string id)
    {
        try
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                return NotFound();

            return Ok(MapToResponseDto(notification));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get current user's notifications
    /// </summary>
    [HttpGet("my-notifications")]
    public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetMyNotifications(
        [FromQuery] bool? isRead = null,
        [FromQuery] int skip = 0,
        [FromQuery] int limit = 50)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var notifications = await _notificationRepository.GetByUserIdAsync(userId, isRead, skip, limit);
            var response = notifications.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get unread notification count
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var count = await _notificationRepository.GetUnreadCountAsync(userId);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                return NotFound();

            if (notification.UserId != userId)
                return Forbid("You can only mark your own notifications as read");

            var marked = await _notificationRepository.MarkAsReadAsync(id);
            if (!marked)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPatch("mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            await _notificationRepository.MarkAllAsReadAsync(userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Delete a notification
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                return NotFound();

            if (notification.UserId != userId)
                return Forbid("You can only delete your own notifications");

            var deleted = await _notificationRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Delete all notifications for current user
    /// </summary>
    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllNotifications()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            await _notificationRepository.DeleteAllForUserAsync(userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting all notifications");
            return StatusCode(500, "An error occurred");
        }
    }

    private static NotificationResponseDto MapToResponseDto(Notification n) => new()
    {
        Id = n.Id!,
        NotificationType = n.NotificationType,
        Title = n.Title,
        Body = n.Body,
        SenderId = n.SenderId,
        ActivityId = n.ActivityId,
        PostId = n.PostId,
        CommentId = n.CommentId,
        ActionUrl = n.ActionUrl,
        DeepLink = n.DeepLink,
        IsRead = n.IsRead,
        ReadAt = n.ReadAt,
        CreatedAt = n.CreatedAt
    };
}
