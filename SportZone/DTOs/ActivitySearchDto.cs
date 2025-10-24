using SportZone.Models;
using System.ComponentModel.DataAnnotations;

namespace SportZone.DTOs;

public class ActivitySearchFilterDto
{
    public SportType? SportType { get; set; }
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
    
    [Range(0.1, 100)]
    public double? RadiusKm { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public bool? HasAvailableSlots { get; set; }
    
    public bool? IsActive { get; set; } = true;
    
    public int? MinParticipants { get; set; }
    
    public int? MaxParticipants { get; set; }
}

public class ActivitySearchResultDto
{
    public string Id { get; set; } = string.Empty;
    public string UniqueId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SportType SportType { get; set; }
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public int? MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public bool IsActive { get; set; }
    public double? DistanceKm { get; set; }
    public int AvailableSlots { get; set; }
    public List<string> Participants { get; set; } = new();
}
