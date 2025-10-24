using SportZone.Models;

namespace SportZone.DTOs;

public class UpdateUserSettingsDto
{
    // Notification preferences
    public bool? EmailNotifications { get; set; }
    public bool? PushNotifications { get; set; }
    public bool? SmsNotifications { get; set; }
    public bool? NotifyOnFollow { get; set; }
    public bool? NotifyOnLike { get; set; }
    public bool? NotifyOnComment { get; set; }
    public bool? NotifyOnInvite { get; set; }
    public bool? NotifyOnMessage { get; set; }
    public bool? NotifyOnActivityReminder { get; set; }
    
    // Privacy
    public ProfileVisibility? ProfileVisibility { get; set; }
    public bool? ShowLocation { get; set; }
    public bool? ShowEmail { get; set; }
    public bool? ShowPhoneNumber { get; set; }
    
    // Activity preferences
    public List<string>? PreferredSports { get; set; }
    public int? MaxDistance { get; set; }
    public List<string>? PreferredDays { get; set; }
    public List<string>? PreferredTimes { get; set; }
    
    // App preferences
    public string? Language { get; set; }
    public Theme? Theme { get; set; }
    public MeasurementUnit? MeasurementUnit { get; set; }
}

public class UserSettingsResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    
    // Notification preferences
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool SmsNotifications { get; set; }
    public bool NotifyOnFollow { get; set; }
    public bool NotifyOnLike { get; set; }
    public bool NotifyOnComment { get; set; }
    public bool NotifyOnInvite { get; set; }
    public bool NotifyOnMessage { get; set; }
    public bool NotifyOnActivityReminder { get; set; }
    
    // Privacy
    public ProfileVisibility ProfileVisibility { get; set; }
    public bool ShowLocation { get; set; }
    public bool ShowEmail { get; set; }
    public bool ShowPhoneNumber { get; set; }
    
    // Activity preferences
    public List<string> PreferredSports { get; set; } = new();
    public int MaxDistance { get; set; }
    public List<string> PreferredDays { get; set; } = new();
    public List<string> PreferredTimes { get; set; } = new();
    
    // App preferences
    public string Language { get; set; } = "en";
    public Theme Theme { get; set; }
    public MeasurementUnit MeasurementUnit { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
