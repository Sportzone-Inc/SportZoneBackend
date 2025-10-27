using Microsoft.AspNetCore.Mvc;
using SportZone.DTOs;
using SportZone.Models;
using SportZone.Services;

namespace SportZone.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _EventService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IEventService EventService, ILogger<EventsController> logger)
    {
        _EventService = EventService;
        _logger = logger;
    }

    /// <summary>
    /// Creëer een nieuwe sportactiviteit
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EventResponseDto>> CreateEvent([FromBody] CreateEventDto createDto)
    {
        try
        {
            var Event = new Event
            {
                Name = createDto.Name,
                Description = createDto.Description,
                SportType = createDto.SportType,
                Location = createDto.Location,
                Latitude = createDto.Latitude,
                Longitude = createDto.Longitude,
                RadiusKm = createDto.RadiusKm,
                ScheduledDate = createDto.ScheduledDate,
                MaxParticipants = createDto.MaxParticipants,
                CreatedBy = createDto.CreatedBy
            };

            var created = await _EventService.CreateEventAsync(Event);
            var response = MapToResponseDto(created);

            return CreatedAtAction(nameof(GetEventById), new { id = created.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sport activity");
            return StatusCode(500, "Er is een fout opgetreden bij het aanmaken van de activiteit");
        }
    }

    /// <summary>
    /// Haal een sportactiviteit op via ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EventResponseDto>> GetEventById(string id)
    {
        var activity = await _EventService.GetEventByIdAsync(id);

        if (activity == null)
        {
            return NotFound($"Activiteit met ID {id} niet gevonden");
        }

        return Ok(MapToResponseDto(activity));
    }

    /// <summary>
    /// Haal een sportactiviteit op via unieke ID
    /// </summary>
    [HttpGet("unique/{uniqueId}")]
    public async Task<ActionResult<EventResponseDto>> GetEventByUniqueId(string uniqueId)
    {
        var activity = await _EventService.GetEventByUniqueIdAsync(uniqueId);

        if (activity == null)
        {
            return NotFound($"Activiteit met unieke ID {uniqueId} niet gevonden");
        }

        return Ok(MapToResponseDto(activity));
    }

    /// <summary>
    /// Haal alle sportactiviteiten op
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetAllevents()
    {
        var activities = await _EventService.GetAlleventsAsync();
        var response = activities.Select(MapToResponseDto);
        return Ok(response);
    }

    /// <summary>
    /// Haal sportactiviteiten op voor een specifieke gebruiker
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<EventResponseDto>>> GeteventsByUser(string userId)
    {
        var activities = await _EventService.GeteventsByUserAsync(userId);
        var response = activities.Select(MapToResponseDto);
        return Ok(response);
    }

    /// <summary>
    /// Haal sportactiviteiten op per sporttype
    /// </summary>
    [HttpGet("type/{sportType}")]
    public async Task<ActionResult<IEnumerable<EventResponseDto>>> GeteventsByType(SportType sportType)
    {
        var activities = await _EventService.GeteventsByTypeAsync(sportType);
        var response = activities.Select(MapToResponseDto);
        return Ok(response);
    }

    /// <summary>
    /// Haal actieve sportactiviteiten op
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetActiveevents()
    {
        var activities = await _EventService.GetActiveeventsAsync();
        var response = activities.Select(MapToResponseDto);
        return Ok(response);
    }

    /// <summary>
    /// Update een sportactiviteit
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateEvent(string id, [FromBody] UpdateEventDto updateDto)
    {
        var existing = await _EventService.GetEventByIdAsync(id);

        if (existing == null)
        {
            return NotFound($"Activiteit met ID {id} niet gevonden");
        }

        existing.Name = updateDto.Name ?? existing.Name;
        existing.Description = updateDto.Description ?? existing.Description;
        existing.SportType = updateDto.SportType ?? existing.SportType;
        existing.Location = updateDto.Location ?? existing.Location;
        existing.Latitude = updateDto.Latitude ?? existing.Latitude;
        existing.Longitude = updateDto.Longitude ?? existing.Longitude;
        existing.RadiusKm = updateDto.RadiusKm ?? existing.RadiusKm;
        existing.ScheduledDate = updateDto.ScheduledDate ?? existing.ScheduledDate;
        existing.MaxParticipants = updateDto.MaxParticipants ?? existing.MaxParticipants;
        existing.IsActive = updateDto.IsActive ?? existing.IsActive;

        var result = await _EventService.UpdateEventAsync(id, existing);

        if (!result)
        {
            return StatusCode(500, "Activiteit bijwerken mislukt");
        }

        return NoContent();
    }

    /// <summary>
    /// Verwijder een sportactiviteit
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEvent(string id)
    {
        var result = await _EventService.DeleteEventAsync(id);

        if (!result)
        {
            return NotFound($"Activiteit met ID {id} niet gevonden");
        }

        return NoContent();
    }

    /// <summary>
    /// Sluit aan bij een sportactiviteit
    /// </summary>
    [HttpPost("{activityId}/join/{userId}")]
    public async Task<ActionResult> JoinEvent(string activityId, string userId)
    {
        try
        {
            var result = await _EventService.JoinEventAsync(activityId, userId);

            if (!result)
            {
                return NotFound("Activiteit niet gevonden");
            }

            return Ok("Succesvol aangesloten bij activiteit");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining activity");
            return StatusCode(500, "Er is een fout opgetreden");
        }
    }

    /// <summary>
    /// Verlaat een sportactiviteit
    /// </summary>
    [HttpPost("{activityId}/leave/{userId}")]
    public async Task<ActionResult> LeaveEvent(string activityId, string userId)
    {
        try
        {
            var result = await _EventService.LeaveEventAsync(activityId, userId);

            if (!result)
            {
                return NotFound("Activiteit niet gevonden");
            }

            return Ok("Succesvol uitgeschreven uit activiteit");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving activity");
            return StatusCode(500, "Er is een fout opgetreden");
        }
    }

    private static EventResponseDto MapToResponseDto(Event activity)
    {
        return new EventResponseDto
        {
            Id = activity.Id!,
            UniqueId = activity.UniqueId,
            Name = activity.Name,
            Description = activity.Description,
            SportType = activity.SportType,
            Location = activity.Location,
            Latitude = activity.Latitude,
            Longitude = activity.Longitude,
            RadiusKm = activity.RadiusKm,
            ScheduledDate = activity.ScheduledDate,
            MaxParticipants = activity.MaxParticipants,
            CurrentParticipants = activity.CurrentParticipants,
            CreatedBy = activity.CreatedBy,
            Participants = activity.Participants,
            IsActive = activity.IsActive,
            CreatedAt = activity.CreatedAt,
            UpdatedAt = activity.UpdatedAt
        };
    }
}
