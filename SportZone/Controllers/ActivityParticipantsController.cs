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
public class ActivityParticipantsController : ControllerBase
{
    private readonly IActivityParticipantRepository _participantRepository;
    private readonly ILogger<ActivityParticipantsController> _logger;

    public ActivityParticipantsController(
        IActivityParticipantRepository participantRepository,
        ILogger<ActivityParticipantsController> logger)
    {
        _participantRepository = participantRepository;
        _logger = logger;
    }

    private string GetCurrentUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    /// <summary>
    /// Join or express interest in an activity
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ParticipantResponseDto>> JoinActivity([FromBody] CreateParticipantDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            // Check if already participating
            var existing = await _participantRepository.GetByActivityAndUserAsync(dto.ActivityId, userId);
            if (existing != null)
                return BadRequest("Already participating in this activity");

            var participant = new ActivityParticipant
            {
                ActivityId = dto.ActivityId,
                UserId = userId,
                Status = dto.Status,
                Notes = dto.Notes
            };

            var created = await _participantRepository.CreateAsync(participant);
            var response = MapToResponseDto(created);

            return CreatedAtAction(nameof(GetParticipant), new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining activity");
            return StatusCode(500, "An error occurred while joining the activity");
        }
    }

    /// <summary>
    /// Get participant by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ParticipantResponseDto>> GetParticipant(string id)
    {
        try
        {
            var participant = await _participantRepository.GetByIdAsync(id);
            if (participant == null)
                return NotFound();

            return Ok(MapToResponseDto(participant));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting participant");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get participants for an activity
    /// </summary>
    [HttpGet("activity/{activityId}")]
    public async Task<ActionResult<IEnumerable<ParticipantResponseDto>>> GetActivityParticipants(
        string activityId,
        [FromQuery] ParticipantStatus? status = null)
    {
        try
        {
            var participants = status.HasValue
                ? await _participantRepository.GetByStatusAsync(activityId, status.Value)
                : await _participantRepository.GetByActivityIdAsync(activityId);

            var response = participants.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting participants");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get current user's participations
    /// </summary>
    [HttpGet("my-participations")]
    public async Task<ActionResult<IEnumerable<ParticipantResponseDto>>> GetMyParticipations()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var participants = await _participantRepository.GetByUserIdAsync(userId);
            var response = participants.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting participations");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get participant count for an activity
    /// </summary>
    [HttpGet("activity/{activityId}/count")]
    public async Task<ActionResult<int>> GetParticipantCount(
        string activityId,
        [FromQuery] ParticipantStatus? status = null)
    {
        try
        {
            var count = await _participantRepository.GetParticipantCountAsync(activityId, status);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting participant count");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Update participation details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateParticipation(string id, [FromBody] UpdateParticipantDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var participant = await _participantRepository.GetByIdAsync(id);
            if (participant == null)
                return NotFound();

            if (participant.UserId != userId)
                return Forbid("You can only update your own participation");

            // Update fields
            if (dto.Status.HasValue) participant.Status = dto.Status.Value;
            if (dto.Notes != null) participant.Notes = dto.Notes;
            if (dto.Rating.HasValue) participant.Rating = dto.Rating;
            if (dto.Review != null) participant.Review = dto.Review;

            var updated = await _participantRepository.UpdateAsync(id, participant);
            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating participation");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Update participation status
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromQuery] ParticipantStatus status)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var participant = await _participantRepository.GetByIdAsync(id);
            if (participant == null)
                return NotFound();

            if (participant.UserId != userId)
                return Forbid("You can only update your own participation");

            var updated = await _participantRepository.UpdateStatusAsync(id, status);
            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Leave/cancel participation
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> LeaveActivity(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var participant = await _participantRepository.GetByIdAsync(id);
            if (participant == null)
                return NotFound();

            if (participant.UserId != userId)
                return Forbid("You can only remove your own participation");

            var deleted = await _participantRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving activity");
            return StatusCode(500, "An error occurred");
        }
    }

    private static ParticipantResponseDto MapToResponseDto(ActivityParticipant p) => new()
    {
        Id = p.Id!,
        ActivityId = p.ActivityId,
        UserId = p.UserId,
        Status = p.Status,
        Role = p.Role,
        JoinedAt = p.JoinedAt,
        CancelledAt = p.CancelledAt,
        AttendanceMarkedAt = p.AttendanceMarkedAt,
        Notes = p.Notes,
        Rating = p.Rating,
        Review = p.Review,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}
