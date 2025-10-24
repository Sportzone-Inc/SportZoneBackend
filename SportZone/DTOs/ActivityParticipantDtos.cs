using System.ComponentModel.DataAnnotations;
using SportZone.Models;

namespace SportZone.DTOs;

public class CreateParticipantDto
{
    [Required]
    public string ActivityId { get; set; } = string.Empty;
    
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Interested;
    public string? Notes { get; set; }
}

public class UpdateParticipantDto
{
    public ParticipantStatus? Status { get; set; }
    public string? Notes { get; set; }
    public int? Rating { get; set; }
    public string? Review { get; set; }
}

public class ParticipantResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string ActivityId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public ParticipantStatus Status { get; set; }
    public ParticipantRole? Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? AttendanceMarkedAt { get; set; }
    public string? Notes { get; set; }
    public int? Rating { get; set; }
    public string? Review { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
