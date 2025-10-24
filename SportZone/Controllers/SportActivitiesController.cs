using Microsoft.AspNetCore.Mvc;
using SportZone.DTOs;
using SportZone.Models;
using SportZone.Services;

namespace SportZone.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SportActivitiesController : ControllerBase
{
    private readonly ISportActivityService _sportActivityService;
    private readonly ILogger<SportActivitiesController> _logger;

    public SportActivitiesController(ISportActivityService sportActivityService, ILogger<SportActivitiesController> logger)
    {
        _sportActivityService = sportActivityService;
        _logger = logger;
    }

    /// <summary>
    /// Creëer een nieuwe sportactiviteit
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SportActivityResponseDto>> CreateSportActivity([FromBody] CreateSportActivityDto createDto)
    {
        try
        {
            var sportActivity = new SportActivity
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

            var created = await _sportActivityService.CreateSportActivityAsync(sportActivity);
            var response = MapToResponseDto(created);

            return CreatedAtAction(nameof(GetSportActivityById), new { id = created.Id }, response);
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
    public async Task<ActionResult<SportActivityResponseDto>> GetSportActivityById(string id)
    {
        var activity = await _sportActivityService.GetSportActivityByIdAsync(id);

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
    public async Task<ActionResult<SportActivityResponseDto>> GetSportActivityByUniqueId(string uniqueId)
    {
        var activity = await _sportActivityService.GetSportActivityByUniqueIdAsync(uniqueId);

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
    public async Task<ActionResult<IEnumerable<SportActivityResponseDto>>> GetAllSportActivities()
    {
        var activities = await _sportActivityService.GetAllSportActivitiesAsync();
        var response = activities.Select(MapToResponseDto);
        return Ok(response);
    }

    /// <summary>
    /// Haal sportactiviteiten op voor een specifieke gebruiker
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<SportActivityResponseDto>>> GetSportActivitiesByUser(string userId)
    {
        var activities = await _sportActivityService.GetSportActivitiesByUserAsync(userId);
        var response = activities.Select(MapToResponseDto);
        return Ok(response);
    }

    /// <summary>
    /// Haal sportactiviteiten op per sporttype
    /// </summary>
    [HttpGet("type/{sportType}")]
    public async Task<ActionResult<IEnumerable<SportActivityResponseDto>>> GetSportActivitiesByType(SportType sportType)
    {
        var activities = await _sportActivityService.GetSportActivitiesByTypeAsync(sportType);
        var response = activities.Select(MapToResponseDto);
        return Ok(response);
    }

    /// <summary>
    /// Haal actieve sportactiviteiten op
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<SportActivityResponseDto>>> GetActiveSportActivities()
    {
        var activities = await _sportActivityService.GetActiveSportActivitiesAsync();
        var response = activities.Select(MapToResponseDto);
        return Ok(response);
    }

    /// <summary>
    /// Zoek activiteiten op basis van locatie en straal
    /// </summary>
    [HttpGet("search/location")]
    public async Task<ActionResult<IEnumerable<ActivitySearchResultDto>>> SearchActivitiesByLocation(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10.0)
    {
        try
        {
            var activities = await _sportActivityService.SearchActivitiesByLocationAsync(latitude, longitude, radiusKm);
            var response = activities.Select(activity => MapToSearchResultDto(activity, latitude, longitude));
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching activities by location");
            return StatusCode(500, "Er is een fout opgetreden bij het zoeken van activiteiten");
        }
    }

    /// <summary>
    /// Zoek activiteiten binnen een datum bereik
    /// </summary>
    [HttpGet("search/daterange")]
    public async Task<ActionResult<IEnumerable<ActivitySearchResultDto>>> SearchActivitiesByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
            {
                return BadRequest("Startdatum moet voor einddatum liggen");
            }

            var activities = await _sportActivityService.SearchActivitiesByDateRangeAsync(startDate, endDate);
            var response = activities.Select(activity => MapToSearchResultDto(activity, null, null));
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching activities by date range");
            return StatusCode(500, "Er is een fout opgetreden bij het zoeken van activiteiten");
        }
    }

    /// <summary>
    /// Geavanceerd zoeken met meerdere filters
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<ActivitySearchResultDto>>> SearchActivitiesWithFilters(
        [FromBody] ActivitySearchFilterDto filters)
    {
        try
        {
            var activities = await _sportActivityService.SearchActivitiesWithFiltersAsync(
                sportType: filters.SportType,
                latitude: filters.Latitude,
                longitude: filters.Longitude,
                radiusKm: filters.RadiusKm,
                startDate: filters.StartDate,
                endDate: filters.EndDate,
                hasAvailableSlots: filters.HasAvailableSlots,
                isActive: filters.IsActive
            );

            var response = activities.Select(activity => 
                MapToSearchResultDto(activity, filters.Latitude, filters.Longitude));
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching activities with filters");
            return StatusCode(500, "Er is een fout opgetreden bij het zoeken van activiteiten");
        }
    }

    /// <summary>
    /// Update een sportactiviteit
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSportActivity(string id, [FromBody] UpdateSportActivityDto updateDto)
    {
        var existing = await _sportActivityService.GetSportActivityByIdAsync(id);

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

        var result = await _sportActivityService.UpdateSportActivityAsync(id, existing);

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
    public async Task<ActionResult> DeleteSportActivity(string id)
    {
        var result = await _sportActivityService.DeleteSportActivityAsync(id);

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
    public async Task<ActionResult> JoinSportActivity(string activityId, string userId)
    {
        try
        {
            var result = await _sportActivityService.JoinSportActivityAsync(activityId, userId);

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
    public async Task<ActionResult> LeaveSportActivity(string activityId, string userId)
    {
        try
        {
            var result = await _sportActivityService.LeaveSportActivityAsync(activityId, userId);

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

    private static SportActivityResponseDto MapToResponseDto(SportActivity activity)
    {
        return new SportActivityResponseDto
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

    private static ActivitySearchResultDto MapToSearchResultDto(SportActivity activity, double? userLat, double? userLon)
    {
        double? distanceKm = null;
        if (userLat.HasValue && userLon.HasValue && activity.Latitude.HasValue && activity.Longitude.HasValue)
        {
            distanceKm = CalculateDistance(userLat.Value, userLon.Value, activity.Latitude.Value, activity.Longitude.Value);
        }

        var availableSlots = activity.MaxParticipants.HasValue
            ? activity.MaxParticipants.Value - activity.CurrentParticipants
            : int.MaxValue;

        return new ActivitySearchResultDto
        {
            Id = activity.Id!,
            UniqueId = activity.UniqueId,
            Name = activity.Name,
            Description = activity.Description,
            SportType = activity.SportType,
            Location = activity.Location,
            Latitude = activity.Latitude,
            Longitude = activity.Longitude,
            ScheduledDate = activity.ScheduledDate,
            MaxParticipants = activity.MaxParticipants,
            CurrentParticipants = activity.CurrentParticipants,
            IsActive = activity.IsActive,
            DistanceKm = distanceKm.HasValue ? Math.Round(distanceKm.Value, 2) : null,
            AvailableSlots = availableSlots,
            Participants = activity.Participants
        };
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
