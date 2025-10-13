using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

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

    [BsonElement("location")]
    public string? Location { get; set; }

    [BsonElement("latitude")]
    public double? Latitude { get; set; }

    [BsonElement("longitude")]
    public double? Longitude { get; set; }

    [BsonElement("radiusKm")]
    public double? RadiusKm { get; set; }

    [BsonElement("scheduledDate")]
    public DateTime? ScheduledDate { get; set; }

    [BsonElement("maxParticipants")]
    public int? MaxParticipants { get; set; }

    [BsonElement("currentParticipants")]
    public int CurrentParticipants { get; set; } = 0;

    [BsonElement("createdBy")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string CreatedBy { get; set; } = string.Empty;

    [BsonElement("participants")]
    public List<string> Participants { get; set; } = new();

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum SportType
{
    Basketball,
    Running,
    Football,
    Tennis,
    Swimming,
    Cycling,
    Volleyball,
    Other
}
