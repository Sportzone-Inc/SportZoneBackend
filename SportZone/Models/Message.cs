using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// Chat messages
/// </summary>
public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("conversationId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string ConversationId { get; set; } = string.Empty;

    [BsonElement("senderId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string SenderId { get; set; } = string.Empty;

    [BsonElement("messageType")]
    [BsonRepresentation(BsonType.String)]
    public MessageType MessageType { get; set; } = MessageType.Text;

    [BsonElement("content")]
    public string? Content { get; set; }

    [BsonElement("mediaUrl")]
    public string? MediaUrl { get; set; }

    [BsonElement("activityId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ActivityId { get; set; }

    [BsonElement("readBy")]
    public List<ReadReceipt> ReadBy { get; set; } = new();

    [BsonElement("isEdited")]
    public bool IsEdited { get; set; } = false;

    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; } = false;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class ReadReceipt
{
    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("readAt")]
    public DateTime ReadAt { get; set; } = DateTime.UtcNow;
}

public enum MessageType
{
    Text,
    Image,
    Video,
    Location,
    Activity,
    System
}
