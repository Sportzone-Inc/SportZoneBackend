using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// Reviews and ratings for activities and users
/// </summary>
public class Review
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("reviewType")]
    [BsonRepresentation(BsonType.String)]
    public ReviewType ReviewType { get; set; } = ReviewType.Activity;

    // References
    [BsonElement("activityId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ActivityId { get; set; }

    [BsonElement("reviewerId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string ReviewerId { get; set; } = string.Empty;

    [BsonElement("revieweeId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? RevieweeId { get; set; }

    [BsonElement("rating")]
    [BsonRequired]
    public int Rating { get; set; } // 1-5

    [BsonElement("title")]
    public string? Title { get; set; }

    [BsonElement("comment")]
    public string? Comment { get; set; }

    // Helpful votes
    [BsonElement("helpfulCount")]
    public int HelpfulCount { get; set; } = 0;

    [BsonElement("helpfulVotes")]
    public List<string> HelpfulVotes { get; set; } = new(); // User ObjectIds

    [BsonElement("isVerified")]
    public bool IsVerified { get; set; } = false;

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("response")]
    public string? Response { get; set; }

    [BsonElement("respondedAt")]
    public DateTime? RespondedAt { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum ReviewType
{
    Activity,
    User
}
