using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// Scheduled sport activities and events
/// </summary>
public class SportActivity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("uniqueId")]
    [BsonRequired]
    public string UniqueId { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("name")]
    [BsonRequired]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("sportType")]
    [BsonRepresentation(BsonType.String)]
    [BsonRequired]
    public SportType SportType { get; set; }

    [BsonElement("category")]
    [BsonRepresentation(BsonType.String)]
    public SportCategory? Category { get; set; }

    // Location data
    [BsonElement("location")]
    public string? Location { get; set; }

    [BsonElement("address")]
    public string? Address { get; set; }

    [BsonElement("city")]
    public string? City { get; set; }

    [BsonElement("country")]
    public string? Country { get; set; }

    [BsonElement("latitude")]
    public double? Latitude { get; set; }

    [BsonElement("longitude")]
    public double? Longitude { get; set; }

    [BsonElement("radiusKm")]
    public double? RadiusKm { get; set; }

    // Scheduling
    [BsonElement("scheduledDate")]
    public DateTime? ScheduledDate { get; set; }

    [BsonElement("startTime")]
    public DateTime? StartTime { get; set; }

    [BsonElement("endTime")]
    public DateTime? EndTime { get; set; }

    [BsonElement("duration")]
    public int? Duration { get; set; } // Duration in minutes

    // Participants
    [BsonElement("maxParticipants")]
    public int? MaxParticipants { get; set; }

    [BsonElement("minParticipants")]
    public int MinParticipants { get; set; } = 1;

    [BsonElement("currentParticipants")]
    public int CurrentParticipants { get; set; } = 0;

    [BsonElement("participants")]
    public List<string> Participants { get; set; } = new(); // Array of user ObjectIds

    [BsonElement("waitlist")]
    public List<string> Waitlist { get; set; } = new(); // Array of user ObjectIds on waitlist

    // Activity details
    [BsonElement("skillLevelRequired")]
    [BsonRepresentation(BsonType.String)]
    public ActivitySkillLevel? SkillLevelRequired { get; set; }

    [BsonElement("ageRestriction")]
    public string? AgeRestriction { get; set; } // e.g., "18+", "All ages"

    [BsonElement("costPerPerson")]
    public decimal CostPerPerson { get; set; } = 0;

    [BsonElement("currency")]
    public string Currency { get; set; } = "USD";

    [BsonElement("equipmentProvided")]
    public bool EquipmentProvided { get; set; } = false;

    [BsonElement("equipmentNeeded")]
    public string? EquipmentNeeded { get; set; }

    // Status and visibility
    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public ActivityStatus Status { get; set; } = ActivityStatus.Draft;

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("isPublic")]
    public bool IsPublic { get; set; } = true;

    [BsonElement("isFeatured")]
    public bool IsFeatured { get; set; } = false;

    // Creator and organizer
    [BsonElement("createdBy")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string CreatedBy { get; set; } = string.Empty;

    [BsonElement("organizerId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? OrganizerId { get; set; }

    // Metadata
    [BsonElement("views")]
    public int Views { get; set; } = 0;

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new(); // Array of tags

    [BsonElement("imageUrls")]
    public List<string> ImageUrls { get; set; } = new(); // Array of image URLs

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Activity skill level requirement
/// </summary>
public enum ActivitySkillLevel
{
    Any,
    Beginner,
    Intermediate,
    Advanced
}

/// <summary>
/// Activity status enumeration
/// </summary>
public enum ActivityStatus
{
    Draft,
    Published,
    InProgress,
    Completed,
    Cancelled
}
