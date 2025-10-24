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
public class ConversationsController : ControllerBase
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<ConversationsController> _logger;

    public ConversationsController(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        ILogger<ConversationsController> logger)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _logger = logger;
    }

    private string GetCurrentUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    /// <summary>
    /// Create a new conversation
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ConversationResponseDto>> CreateConversation([FromBody] CreateConversationDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            // For direct conversations, check if one already exists
            if (dto.ConversationType == ConversationType.Direct && dto.Participants.Count == 2)
            {
                var existing = await _conversationRepository.GetDirectConversationAsync(
                    dto.Participants[0], dto.Participants[1]);
                if (existing != null)
                    return Ok(await MapToResponseDtoAsync(existing, userId));
            }

            var conversation = new Conversation
            {
                Participants = dto.Participants,
                ConversationType = dto.ConversationType,
                ActivityId = dto.ActivityId,
                Name = dto.Name,
                ImageUrl = dto.ImageUrl,
                CreatedBy = userId
            };

            var created = await _conversationRepository.CreateAsync(conversation);
            var response = await MapToResponseDtoAsync(created, userId);

            return CreatedAtAction(nameof(GetConversation), new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conversation");
            return StatusCode(500, "An error occurred while creating the conversation");
        }
    }

    /// <summary>
    /// Get conversation by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ConversationResponseDto>> GetConversation(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var conversation = await _conversationRepository.GetByIdAsync(id);
            if (conversation == null)
                return NotFound();

            if (!conversation.Participants.Contains(userId))
                return Forbid("You are not a participant in this conversation");

            var response = await MapToResponseDtoAsync(conversation, userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversation");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get current user's conversations
    /// </summary>
    [HttpGet("my-conversations")]
    public async Task<ActionResult<IEnumerable<ConversationResponseDto>>> GetMyConversations()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var conversations = await _conversationRepository.GetByUserIdAsync(userId);
            var response = new List<ConversationResponseDto>();

            foreach (var conv in conversations)
            {
                response.Add(await MapToResponseDtoAsync(conv, userId));
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversations");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Update conversation details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateConversation(string id, [FromBody] UpdateConversationDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var conversation = await _conversationRepository.GetByIdAsync(id);
            if (conversation == null)
                return NotFound();

            if (!conversation.Participants.Contains(userId))
                return Forbid("You are not a participant in this conversation");

            if (dto.Name != null) conversation.Name = dto.Name;
            if (dto.ImageUrl != null) conversation.ImageUrl = dto.ImageUrl;

            var updated = await _conversationRepository.UpdateAsync(id, conversation);
            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating conversation");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Delete a conversation
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConversation(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var conversation = await _conversationRepository.GetByIdAsync(id);
            if (conversation == null)
                return NotFound();

            if (conversation.CreatedBy != userId)
                return Forbid("Only the creator can delete this conversation");

            var deleted = await _conversationRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conversation");
            return StatusCode(500, "An error occurred");
        }
    }

    private async Task<ConversationResponseDto> MapToResponseDtoAsync(Conversation c, string userId)
    {
        var unreadCount = await _messageRepository.GetUnreadCountAsync(c.Id!, userId);
        
        return new ConversationResponseDto
        {
            Id = c.Id!,
            Participants = c.Participants,
            ConversationType = c.ConversationType,
            ActivityId = c.ActivityId,
            Name = c.Name,
            ImageUrl = c.ImageUrl,
            LastMessageId = c.LastMessageId,
            LastMessageAt = c.LastMessageAt,
            CreatedAt = c.CreatedAt,
            UnreadCount = unreadCount
        };
    }
}
