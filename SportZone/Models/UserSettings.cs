using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// User preferences and settings
/// </summary>
public class UserSettings
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonRequired]
    public string UserId { get; set; } = string.Empty;

    // Notification preferences
    [BsonElement("emailNotifications")]
    public bool EmailNotifications { get; set; } = true;

    [BsonElement("pushNotifications")]
    public bool PushNotifications { get; set; } = true;

    [BsonElement("smsNotifications")]
    public bool SmsNotifications { get; set; } = false;

    // Notification types
    [BsonElement("notifyOnFollow")]
    public bool NotifyOnFollow { get; set; } = true;

    [BsonElement("notifyOnLike")]
    public bool NotifyOnLike { get; set; } = true;

    [BsonElement("notifyOnComment")]
    public bool NotifyOnComment { get; set; } = true;

    [BsonElement("notifyOnInvite")]
    public bool NotifyOnInvite { get; set; } = true;

    [BsonElement("notifyOnMessage")]
    public bool NotifyOnMessage { get; set; } = true;

    [BsonElement("notifyOnActivityReminder")]
    public bool NotifyOnActivityReminder { get; set; } = true;

    // Privacy
    [BsonElement("profileVisibility")]
    [BsonRepresentation(BsonType.String)]
    public ProfileVisibility ProfileVisibility { get; set; } = ProfileVisibility.Public;

    [BsonElement("showLocation")]
    public bool ShowLocation { get; set; } = true;

    [BsonElement("showEmail")]
    public bool ShowEmail { get; set; } = false;

    [BsonElement("showPhoneNumber")]
    public bool ShowPhoneNumber { get; set; } = false;

    // Activity preferences
    [BsonElement("preferredSports")]
    public List<string> PreferredSports { get; set; } = new();

    [BsonElement("maxDistance")]
    public int MaxDistance { get; set; } = 50; // in km

    [BsonElement("preferredDays")]
    public List<string> PreferredDays { get; set; } = new();

    [BsonElement("preferredTimes")]
    public List<string> PreferredTimes { get; set; } = new();

    // App preferences
    [BsonElement("language")]
    public string Language { get; set; } = "en";

    [BsonElement("theme")]
    [BsonRepresentation(BsonType.String)]
    public Theme Theme { get; set; } = Theme.Light;

    [BsonElement("measurementUnit")]
    [BsonRepresentation(BsonType.String)]
    public MeasurementUnit MeasurementUnit { get; set; } = MeasurementUnit.Metric;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum ProfileVisibility
{
    Public,
    Friends,
    Private
}

public enum Theme
{
    Light,
    Dark,
    Auto
}

public enum MeasurementUnit
{
    Metric,
    Imperial
}
