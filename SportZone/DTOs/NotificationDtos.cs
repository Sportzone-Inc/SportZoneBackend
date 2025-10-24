using System.ComponentModel.DataAnnotations;
using SportZone.Models;

namespace SportZone.DTOs;

public class CreateNotificationDto
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public NotificationType NotificationType { get; set; }
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    public string? Body { get; set; }
    public string? SenderId { get; set; }
    public string? ActivityId { get; set; }
    public string? PostId { get; set; }
    public string? CommentId { get; set; }
    public string? ActionUrl { get; set; }
    public string? DeepLink { get; set; }
}

public class NotificationResponseDto
{
    public string Id { get; set; } = string.Empty;
    public NotificationType NotificationType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string? SenderId { get; set; }
    public string? ActivityId { get; set; }
    public string? PostId { get; set; }
    public string? CommentId { get; set; }
    public string? ActionUrl { get; set; }
    public string? DeepLink { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
