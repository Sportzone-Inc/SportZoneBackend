using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportZone.DTOs;
using SportZone.Models;
using SportZone.Repositories;
using System.Security.Claims;

namespace SportZone.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserSettingsController : ControllerBase
{
    private readonly IUserSettingsRepository _settingsRepository;
    private readonly ILogger<UserSettingsController> _logger;

    public UserSettingsController(
        IUserSettingsRepository settingsRepository,
        ILogger<UserSettingsController> logger)
    {
        _settingsRepository = settingsRepository;
        _logger = logger;
    }

    private string GetCurrentUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    /// <summary>
    /// Get current user's settings
    /// </summary>
    [HttpGet("my-settings")]
    public async Task<ActionResult<UserSettingsResponseDto>> GetMySettings()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var settings = await _settingsRepository.GetByUserIdAsync(userId);
            
            // Create default settings if they don't exist
            if (settings == null)
            {
                settings = new UserSettings { UserId = userId };
                await _settingsRepository.CreateAsync(settings);
            }

            return Ok(MapToResponseDto(settings));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting settings");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get settings by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<UserSettingsResponseDto>> GetUserSettings(string userId)
    {
        try
        {
            var settings = await _settingsRepository.GetByUserIdAsync(userId);
            if (settings == null)
                return NotFound();

            return Ok(MapToResponseDto(settings));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user settings");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Update current user's settings
    /// </summary>
    [HttpPut("my-settings")]
    public async Task<IActionResult> UpdateMySettings([FromBody] UpdateUserSettingsDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var settings = await _settingsRepository.GetByUserIdAsync(userId);
            
            if (settings == null)
            {
                // Create new settings if they don't exist
                settings = new UserSettings { UserId = userId };
                ApplyUpdates(settings, dto);
                await _settingsRepository.CreateAsync(settings);
                return NoContent();
            }

            ApplyUpdates(settings, dto);
            var updated = await _settingsRepository.UpdateAsync(settings.Id!, settings);
            
            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating settings");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Reset settings to default
    /// </summary>
    [HttpPost("reset")]
    public async Task<IActionResult> ResetSettings()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var settings = await _settingsRepository.GetByUserIdAsync(userId);
            if (settings == null)
                return NotFound();

            // Delete existing and create new with defaults
            await _settingsRepository.DeleteAsync(settings.Id!);
            
            var newSettings = new UserSettings { UserId = userId };
            await _settingsRepository.CreateAsync(newSettings);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting settings");
            return StatusCode(500, "An error occurred");
        }
    }

    private void ApplyUpdates(UserSettings settings, UpdateUserSettingsDto dto)
    {
        // Notification preferences
        if (dto.EmailNotifications.HasValue) settings.EmailNotifications = dto.EmailNotifications.Value;
        if (dto.PushNotifications.HasValue) settings.PushNotifications = dto.PushNotifications.Value;
        if (dto.SmsNotifications.HasValue) settings.SmsNotifications = dto.SmsNotifications.Value;
        if (dto.NotifyOnFollow.HasValue) settings.NotifyOnFollow = dto.NotifyOnFollow.Value;
        if (dto.NotifyOnLike.HasValue) settings.NotifyOnLike = dto.NotifyOnLike.Value;
        if (dto.NotifyOnComment.HasValue) settings.NotifyOnComment = dto.NotifyOnComment.Value;
        if (dto.NotifyOnInvite.HasValue) settings.NotifyOnInvite = dto.NotifyOnInvite.Value;
        if (dto.NotifyOnMessage.HasValue) settings.NotifyOnMessage = dto.NotifyOnMessage.Value;
        if (dto.NotifyOnActivityReminder.HasValue) settings.NotifyOnActivityReminder = dto.NotifyOnActivityReminder.Value;

        // Privacy
        if (dto.ProfileVisibility.HasValue) settings.ProfileVisibility = dto.ProfileVisibility.Value;
        if (dto.ShowLocation.HasValue) settings.ShowLocation = dto.ShowLocation.Value;
        if (dto.ShowEmail.HasValue) settings.ShowEmail = dto.ShowEmail.Value;
        if (dto.ShowPhoneNumber.HasValue) settings.ShowPhoneNumber = dto.ShowPhoneNumber.Value;

        // Activity preferences
        if (dto.PreferredSports != null) settings.PreferredSports = dto.PreferredSports;
        if (dto.MaxDistance.HasValue) settings.MaxDistance = dto.MaxDistance.Value;
        if (dto.PreferredDays != null) settings.PreferredDays = dto.PreferredDays;
        if (dto.PreferredTimes != null) settings.PreferredTimes = dto.PreferredTimes;

        // App preferences
        if (dto.Language != null) settings.Language = dto.Language;
        if (dto.Theme.HasValue) settings.Theme = dto.Theme.Value;
        if (dto.MeasurementUnit.HasValue) settings.MeasurementUnit = dto.MeasurementUnit.Value;
    }

    private static UserSettingsResponseDto MapToResponseDto(UserSettings s) => new()
    {
        Id = s.Id!,
        UserId = s.UserId,
        EmailNotifications = s.EmailNotifications,
        PushNotifications = s.PushNotifications,
        SmsNotifications = s.SmsNotifications,
        NotifyOnFollow = s.NotifyOnFollow,
        NotifyOnLike = s.NotifyOnLike,
        NotifyOnComment = s.NotifyOnComment,
        NotifyOnInvite = s.NotifyOnInvite,
        NotifyOnMessage = s.NotifyOnMessage,
        NotifyOnActivityReminder = s.NotifyOnActivityReminder,
        ProfileVisibility = s.ProfileVisibility,
        ShowLocation = s.ShowLocation,
        ShowEmail = s.ShowEmail,
        ShowPhoneNumber = s.ShowPhoneNumber,
        PreferredSports = s.PreferredSports,
        MaxDistance = s.MaxDistance,
        PreferredDays = s.PreferredDays,
        PreferredTimes = s.PreferredTimes,
        Language = s.Language,
        Theme = s.Theme,
        MeasurementUnit = s.MeasurementUnit,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt
    };
}
