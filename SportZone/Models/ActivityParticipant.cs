using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// User participation in activities
/// </summary>
public class ActivityParticipant
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("activityId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string ActivityId { get; set; } = string.Empty;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    [BsonRequired]
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Interested;

    [BsonElement("role")]
    [BsonRepresentation(BsonType.String)]
    public ParticipantRole? Role { get; set; }

    [BsonElement("joinedAt")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("cancelledAt")]
    public DateTime? CancelledAt { get; set; }

    [BsonElement("attendanceMarkedAt")]
    public DateTime? AttendanceMarkedAt { get; set; }

    [BsonElement("notes")]
    public string? Notes { get; set; }

    [BsonElement("rating")]
    public int? Rating { get; set; } // 1-5

    [BsonElement("review")]
    public string? Review { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum ParticipantStatus
{
    Interested,
    Joined,
    Attended,
    Cancelled,
    NoShow
}

public enum ParticipantRole
{
    Participant,
    Organizer,
    CoOrganizer
}
