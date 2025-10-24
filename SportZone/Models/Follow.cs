using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// User follow relationships
/// </summary>
public class Follow
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("followerId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string FollowerId { get; set; } = string.Empty;

    [BsonElement("followingId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string FollowingId { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
