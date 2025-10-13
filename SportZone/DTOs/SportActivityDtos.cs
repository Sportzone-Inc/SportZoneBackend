using System.ComponentModel.DataAnnotations;
using SportZone.Models;

namespace SportZone.DTOs;

public class CreateSportActivityDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public SportType SportType { get; set; }

    public string? Location { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public double? RadiusKm { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public int? MaxParticipants { get; set; }

    [Required]
    public string CreatedBy { get; set; } = string.Empty;
}

public class UpdateSportActivityDto
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public SportType? SportType { get; set; }

    public string? Location { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public double? RadiusKm { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public int? MaxParticipants { get; set; }

    public bool? IsActive { get; set; }
}

public class SportActivityResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string UniqueId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SportType SportType { get; set; }
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusKm { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public int? MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<string> Participants { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
