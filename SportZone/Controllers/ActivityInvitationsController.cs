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
public class ActivityInvitationsController : ControllerBase
{
    private readonly IActivityInvitationRepository _invitationRepository;
    private readonly ILogger<ActivityInvitationsController> _logger;

    public ActivityInvitationsController(
        IActivityInvitationRepository invitationRepository,
        ILogger<ActivityInvitationsController> logger)
    {
        _invitationRepository = invitationRepository;
        _logger = logger;
    }

    private string GetCurrentUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    /// <summary>
    /// Send an activity invitation
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<InvitationResponseDto>> SendInvitation([FromBody] CreateInvitationDto dto)
    {
        try
        {
            var senderId = GetCurrentUserId();
            if (string.IsNullOrEmpty(senderId))
                return Unauthorized("User not authenticated");

            if (senderId == dto.ReceiverId)
                return BadRequest("Cannot invite yourself");

            var invitation = new ActivityInvitation
            {
                ActivityId = dto.ActivityId,
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Message = dto.Message,
                ExpiresAt = dto.ExpiresAt
            };

            var created = await _invitationRepository.CreateAsync(invitation);
            var response = MapToResponseDto(created);

            return CreatedAtAction(nameof(GetInvitation), new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending invitation");
            return StatusCode(500, "An error occurred while sending the invitation");
        }
    }

    /// <summary>
    /// Get invitation by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<InvitationResponseDto>> GetInvitation(string id)
    {
        try
        {
            var invitation = await _invitationRepository.GetByIdAsync(id);
            if (invitation == null)
                return NotFound();

            return Ok(MapToResponseDto(invitation));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invitation");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get current user's received invitations
    /// </summary>
    [HttpGet("my-invitations")]
    public async Task<ActionResult<IEnumerable<InvitationResponseDto>>> GetMyInvitations(
        [FromQuery] InvitationStatus? status = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var invitations = status.HasValue
                ? await _invitationRepository.GetByStatusAsync(userId, status.Value)
                : await _invitationRepository.GetByReceiverIdAsync(userId);

            var response = invitations.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invitations");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get invitations for an activity
    /// </summary>
    [HttpGet("activity/{activityId}")]
    public async Task<ActionResult<IEnumerable<InvitationResponseDto>>> GetActivityInvitations(string activityId)
    {
        try
        {
            var invitations = await _invitationRepository.GetByActivityIdAsync(activityId);
            var response = invitations.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting activity invitations");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Respond to an invitation (accept/decline)
    /// </summary>
    [HttpPatch("{id}/respond")]
    public async Task<IActionResult> RespondToInvitation(string id, [FromBody] RespondToInvitationDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var invitation = await _invitationRepository.GetByIdAsync(id);
            if (invitation == null)
                return NotFound();

            if (invitation.ReceiverId != userId)
                return Forbid("You can only respond to your own invitations");

            if (invitation.Status != InvitationStatus.Pending)
                return BadRequest("Invitation has already been responded to");

            var updated = await _invitationRepository.UpdateStatusAsync(id, dto.Status);
            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error responding to invitation");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Cancel/delete an invitation
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvitation(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var invitation = await _invitationRepository.GetByIdAsync(id);
            if (invitation == null)
                return NotFound();

            if (invitation.SenderId != userId)
                return Forbid("You can only delete invitations you sent");

            var deleted = await _invitationRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting invitation");
            return StatusCode(500, "An error occurred");
        }
    }

    private static InvitationResponseDto MapToResponseDto(ActivityInvitation i) => new()
    {
        Id = i.Id!,
        ActivityId = i.ActivityId,
        SenderId = i.SenderId,
        ReceiverId = i.ReceiverId,
        Status = i.Status,
        Message = i.Message,
        SentAt = i.SentAt,
        RespondedAt = i.RespondedAt,
        ExpiresAt = i.ExpiresAt
    };
}
