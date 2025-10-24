using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// Comments on posts
/// </summary>
public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("postId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string PostId { get; set; } = string.Empty;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("parentCommentId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ParentCommentId { get; set; }

    [BsonElement("body")]
    [BsonRequired]
    public string Body { get; set; } = string.Empty;

    // Engagement
    [BsonElement("likes")]
    public List<string> Likes { get; set; } = new(); // Array of user ObjectIds who liked

    [BsonElement("likesCount")]
    public int LikesCount { get; set; } = 0;

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("isEdited")]
    public bool IsEdited { get; set; } = false;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
