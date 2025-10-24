using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// Sport types and configurations
/// </summary>
public class Sport
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    [BsonRequired]
    public string Name { get; set; } = string.Empty;

    [BsonElement("type")]
    [BsonRepresentation(BsonType.String)]
    [BsonRequired]
    public SportType Type { get; set; }

    [BsonElement("category")]
    [BsonRepresentation(BsonType.String)]
    public SportCategory? Category { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("iconUrl")]
    public string? IconUrl { get; set; }

    [BsonElement("location")]
    public string? Location { get; set; }

    [BsonElement("radiusKm")]
    public double? RadiusKm { get; set; }

    [BsonElement("createdBy")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CreatedBy { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Sport type enumeration
/// </summary>
public enum SportType
{
    Basketball,
    Running,
    Football,
    Tennis,
    Soccer,
    Volleyball,
    Swimming,
    Cycling,
    Yoga,
    Golf,
    Baseball,
    Badminton,
    TableTennis,
    Climbing,
    Hiking,
    Skiing,
    Surfing,
    Boxing,
    MartialArts,
    Gym,
    Other
}

/// <summary>
/// Sport category enumeration
/// </summary>
public enum SportCategory
{
    Team,
    Individual,
    Water,
    Combat,
    Racquet,
    Fitness,
    Outdoor,
    Indoor,
    Extreme,
    Winter,
    Other
}
