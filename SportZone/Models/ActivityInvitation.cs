using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// Invitations to join activities
/// </summary>
public class ActivityInvitation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("activityId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string ActivityId { get; set; } = string.Empty;

    [BsonElement("senderId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string SenderId { get; set; } = string.Empty;

    [BsonElement("receiverId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string ReceiverId { get; set; } = string.Empty;

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;

    [BsonElement("message")]
    public string? Message { get; set; }

    [BsonElement("sentAt")]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    [BsonElement("respondedAt")]
    public DateTime? RespondedAt { get; set; }

    [BsonElement("expiresAt")]
    public DateTime? ExpiresAt { get; set; }
}

public enum InvitationStatus
{
    Pending,
    Accepted,
    Declined,
    Expired
}
