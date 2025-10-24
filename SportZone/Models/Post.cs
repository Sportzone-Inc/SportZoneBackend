using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// User posts and updates
/// </summary>
public class Post
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("activityId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ActivityId { get; set; }

    // Content
    [BsonElement("title")]
    public string? Title { get; set; }

    [BsonElement("body")]
    [BsonRequired]
    public string Body { get; set; } = string.Empty;

    [BsonElement("postType")]
    [BsonRepresentation(BsonType.String)]
    public PostType PostType { get; set; } = PostType.Text;

    // Media
    [BsonElement("mediaUrls")]
    public List<string> MediaUrls { get; set; } = new();

    [BsonElement("thumbnailUrl")]
    public string? ThumbnailUrl { get; set; }

    // Engagement
    [BsonElement("likes")]
    public List<string> Likes { get; set; } = new(); // Array of user ObjectIds who liked

    [BsonElement("likesCount")]
    public int LikesCount { get; set; } = 0;

    [BsonElement("commentsCount")]
    public int CommentsCount { get; set; } = 0;

    [BsonElement("sharesCount")]
    public int SharesCount { get; set; } = 0;

    // Visibility
    [BsonElement("visibility")]
    [BsonRepresentation(BsonType.String)]
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("isPinned")]
    public bool IsPinned { get; set; } = false;

    // Metadata
    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new(); // Array of hashtags

    [BsonElement("location")]
    public string? Location { get; set; }

    [BsonElement("latitude")]
    public double? Latitude { get; set; }

    [BsonElement("longitude")]
    public double? Longitude { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Post type enumeration
/// </summary>
public enum PostType
{
    Text,
    Image,
    Video,
    Activity,
    Achievement
}

/// <summary>
/// Post visibility enumeration
/// </summary>
public enum PostVisibility
{
    Public,
    Friends,
    Private
}
