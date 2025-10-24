using System.ComponentModel.DataAnnotations;
using SportZone.Models;

namespace SportZone.DTOs;

public class CreateInvitationDto
{
    [Required]
    public string ActivityId { get; set; } = string.Empty;
    
    [Required]
    public string ReceiverId { get; set; } = string.Empty;
    
    public string? Message { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class RespondToInvitationDto
{
    [Required]
    public InvitationStatus Status { get; set; }
}

public class InvitationResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string ActivityId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public InvitationStatus Status { get; set; }
    public string? Message { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
