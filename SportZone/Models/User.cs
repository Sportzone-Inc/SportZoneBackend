using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// User accounts for SportZone platform
/// </summary>
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("username")]
    public string? Username { get; set; }

    [BsonElement("email")]
    [BsonRequired]
    public string Email { get; set; } = string.Empty;

    [BsonElement("password")]
    [BsonRequired]
    public string Password { get; set; } = string.Empty;

    [BsonElement("firstName")]
    public string? FirstName { get; set; }

    [BsonElement("lastName")]
    public string? LastName { get; set; }

    [BsonElement("name")]
    [BsonRequired]
    public string Name { get; set; } = string.Empty;

    [BsonElement("profileImage")]
    public string? ProfileImage { get; set; }

    [BsonElement("bio")]
    public string? Bio { get; set; }

    [BsonElement("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [BsonElement("dateOfBirth")]
    public DateTime? DateOfBirth { get; set; }

    [BsonElement("gender")]
    [BsonRepresentation(BsonType.String)]
    public Gender? Gender { get; set; }

    [BsonElement("location")]
    public string? Location { get; set; }

    [BsonElement("latitude")]
    public double? Latitude { get; set; }

    [BsonElement("longitude")]
    public double? Longitude { get; set; }

    [BsonElement("preferredSport")]
    public string? PreferredSport { get; set; }

    [BsonElement("skillLevel")]
    [BsonRepresentation(BsonType.String)]
    public SkillLevel? SkillLevel { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("isVerified")]
    public bool IsVerified { get; set; } = false;

    [BsonElement("lastLoginAt")]
    public DateTime? LastLoginAt { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Gender enumeration
/// </summary>
public enum Gender
{
    Male,
    Female,
    Other,
    PreferNotToSay
}

/// <summary>
/// Skill level enumeration
/// </summary>
public enum SkillLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}
