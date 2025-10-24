using System.ComponentModel.DataAnnotations;
using SportZone.Models;

namespace SportZone.DTOs;

public class CreateConversationDto
{
    [Required]
    public List<string> Participants { get; set; } = new();
    
    public ConversationType ConversationType { get; set; } = ConversationType.Direct;
    public string? ActivityId { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateConversationDto
{
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
}

public class ConversationResponseDto
{
    public string Id { get; set; } = string.Empty;
    public List<string> Participants { get; set; } = new();
    public ConversationType ConversationType { get; set; }
    public string? ActivityId { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? LastMessageId { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UnreadCount { get; set; }
}
