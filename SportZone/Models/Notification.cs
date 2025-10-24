using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// User notifications
/// </summary>
public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("notificationType")]
    [BsonRepresentation(BsonType.String)]
    [BsonRequired]
    public NotificationType NotificationType { get; set; }

    [BsonElement("title")]
    [BsonRequired]
    public string Title { get; set; } = string.Empty;

    [BsonElement("body")]
    public string? Body { get; set; }

    // References
    [BsonElement("senderId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? SenderId { get; set; }

    [BsonElement("activityId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ActivityId { get; set; }

    [BsonElement("postId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? PostId { get; set; }

    [BsonElement("commentId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CommentId { get; set; }

    // Navigation
    [BsonElement("actionUrl")]
    public string? ActionUrl { get; set; }

    [BsonElement("deepLink")]
    public string? DeepLink { get; set; }

    // Status
    [BsonElement("isRead")]
    public bool IsRead { get; set; } = false;

    [BsonElement("readAt")]
    public DateTime? ReadAt { get; set; }

    [BsonElement("isSent")]
    public bool IsSent { get; set; } = false;

    [BsonElement("sentAt")]
    public DateTime? SentAt { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum NotificationType
{
    Follow,
    Like,
    Comment,
    ActivityInvite,
    ActivityReminder,
    ActivityUpdate,
    ActivityCancelled,
    Message,
    NewPost,
    Mention,
    System
}
