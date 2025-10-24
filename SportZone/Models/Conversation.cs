using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// Chat conversations
/// </summary>
public class Conversation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("participants")]
    [BsonRequired]
    public List<string> Participants { get; set; } = new(); // User ObjectIds

    [BsonElement("conversationType")]
    [BsonRepresentation(BsonType.String)]
    public ConversationType ConversationType { get; set; } = ConversationType.Direct;

    [BsonElement("activityId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ActivityId { get; set; }

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("imageUrl")]
    public string? ImageUrl { get; set; }

    [BsonElement("lastMessageId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? LastMessageId { get; set; }

    [BsonElement("lastMessageAt")]
    public DateTime? LastMessageAt { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("createdBy")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CreatedBy { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum ConversationType
{
    Direct,
    Group,
    Activity
}
