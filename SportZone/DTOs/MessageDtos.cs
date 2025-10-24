using System.ComponentModel.DataAnnotations;
using SportZone.Models;

namespace SportZone.DTOs;

public class CreateMessageDto
{
    [Required]
    public string ConversationId { get; set; } = string.Empty;
    
    public MessageType MessageType { get; set; } = MessageType.Text;
    public string? Content { get; set; }
    public string? MediaUrl { get; set; }
    public string? ActivityId { get; set; }
}

public class UpdateMessageDto
{
    [Required]
    public string Content { get; set; } = string.Empty;
}

public class MessageResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public MessageType MessageType { get; set; }
    public string? Content { get; set; }
    public string? MediaUrl { get; set; }
    public string? ActivityId { get; set; }
    public List<ReadReceiptDto> ReadBy { get; set; } = new();
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ReadReceiptDto
{
    public string UserId { get; set; } = string.Empty;
    public DateTime ReadAt { get; set; }
}
