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
public class MessagesController : ControllerBase
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(
        IMessageRepository messageRepository,
        IConversationRepository conversationRepository,
        ILogger<MessagesController> logger)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _logger = logger;
    }

    private string GetCurrentUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    /// <summary>
    /// Send a message
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MessageResponseDto>> SendMessage([FromBody] CreateMessageDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            // Verify user is in conversation
            var conversation = await _conversationRepository.GetByIdAsync(dto.ConversationId);
            if (conversation == null)
                return NotFound("Conversation not found");

            if (!conversation.Participants.Contains(userId))
                return Forbid("You are not a participant in this conversation");

            var message = new Message
            {
                ConversationId = dto.ConversationId,
                SenderId = userId,
                MessageType = dto.MessageType,
                Content = dto.Content,
                MediaUrl = dto.MediaUrl,
                ActivityId = dto.ActivityId
            };

            var created = await _messageRepository.CreateAsync(message);

            // Update conversation's last message
            await _conversationRepository.UpdateLastMessageAsync(
                dto.ConversationId, created.Id!, created.CreatedAt);

            var response = MapToResponseDto(created);
            return CreatedAtAction(nameof(GetMessage), new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            return StatusCode(500, "An error occurred while sending the message");
        }
    }

    /// <summary>
    /// Get message by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MessageResponseDto>> GetMessage(string id)
    {
        try
        {
            var message = await _messageRepository.GetByIdAsync(id);
            if (message == null)
                return NotFound();

            return Ok(MapToResponseDto(message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting message");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get messages for a conversation
    /// </summary>
    [HttpGet("conversation/{conversationId}")]
    public async Task<ActionResult<IEnumerable<MessageResponseDto>>> GetConversationMessages(
        string conversationId,
        [FromQuery] int skip = 0,
        [FromQuery] int limit = 50)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            // Verify user is in conversation
            var conversation = await _conversationRepository.GetByIdAsync(conversationId);
            if (conversation == null)
                return NotFound("Conversation not found");

            if (!conversation.Participants.Contains(userId))
                return Forbid("You are not a participant in this conversation");

            var messages = await _messageRepository.GetByConversationIdAsync(conversationId, skip, limit);
            var response = messages.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting messages");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Update a message
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMessage(string id, [FromBody] UpdateMessageDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var message = await _messageRepository.GetByIdAsync(id);
            if (message == null)
                return NotFound();

            if (message.SenderId != userId)
                return Forbid("You can only edit your own messages");

            message.Content = dto.Content;
            var updated = await _messageRepository.UpdateAsync(id, message);
            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating message");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Delete a message
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var message = await _messageRepository.GetByIdAsync(id);
            if (message == null)
                return NotFound();

            if (message.SenderId != userId)
                return Forbid("You can only delete your own messages");

            var deleted = await _messageRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting message");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Mark a message as read
    /// </summary>
    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var marked = await _messageRepository.MarkAsReadAsync(id, userId);
            if (!marked)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message as read");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Mark all messages in a conversation as read
    /// </summary>
    [HttpPost("conversation/{conversationId}/read-all")]
    public async Task<IActionResult> MarkConversationAsRead(string conversationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var marked = await _messageRepository.MarkConversationAsReadAsync(conversationId, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking conversation as read");
            return StatusCode(500, "An error occurred");
        }
    }

    private static MessageResponseDto MapToResponseDto(Message m) => new()
    {
        Id = m.Id!,
        ConversationId = m.ConversationId,
        SenderId = m.SenderId,
        MessageType = m.MessageType,
        Content = m.Content,
        MediaUrl = m.MediaUrl,
        ActivityId = m.ActivityId,
        ReadBy = m.ReadBy.Select(r => new ReadReceiptDto { UserId = r.UserId, ReadAt = r.ReadAt }).ToList(),
        IsEdited = m.IsEdited,
        CreatedAt = m.CreatedAt,
        UpdatedAt = m.UpdatedAt
    };
}
