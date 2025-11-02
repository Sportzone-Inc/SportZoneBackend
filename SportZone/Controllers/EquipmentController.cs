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
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly ILogger<EquipmentController> _logger;

    public EquipmentController(
        IEquipmentRepository equipmentRepository,
        ILogger<EquipmentController> logger)
    {
        _equipmentRepository = equipmentRepository;
        _logger = logger;
    }

    private string GetCurrentUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    /// <summary>
    /// Create new equipment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EquipmentResponseDto>> CreateEquipment([FromBody] CreateEquipmentDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var equipment = new Equipment
            {
                Name = dto.Name,
                Type = dto.Type,
                Category = dto.Category,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Brand = dto.Brand,
                Model = dto.Model,
                Sku = dto.Sku,
                Quantity = dto.Quantity,
                Condition = dto.Condition,
                PurchasePrice = dto.PurchasePrice,
                RentalPricePerDay = dto.RentalPricePerDay,
                Currency = dto.Currency,
                OwnerId = userId,
                LocationId = dto.LocationId,
                AvailableForRent = dto.AvailableForRent,
                AvailableForSale = dto.AvailableForSale,
                MaintenanceDate = dto.MaintenanceDate,
                PurchaseDate = dto.PurchaseDate,
                Tags = dto.Tags ?? new List<string>(),
                Specifications = dto.Specifications ?? new Dictionary<string, string>()
            };

            var created = await _equipmentRepository.CreateAsync(equipment);
            var response = MapToResponseDto(created);

            return CreatedAtAction(nameof(GetEquipment), new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating equipment");
            return StatusCode(500, "An error occurred while creating the equipment");
        }
    }

    /// <summary>
    /// Get equipment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EquipmentResponseDto>> GetEquipment(string id)
    {
        try
        {
            var equipment = await _equipmentRepository.GetByIdAsync(id);
            if (equipment == null)
                return NotFound();

            return Ok(MapToResponseDto(equipment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting equipment");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get all equipment
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquipmentResponseDto>>> GetAllEquipment()
    {
        try
        {
            var equipment = await _equipmentRepository.GetAllAsync();
            var response = equipment.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all equipment");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get equipment by owner
    /// </summary>
    [HttpGet("owner/{ownerId}")]
    public async Task<ActionResult<IEnumerable<EquipmentResponseDto>>> GetEquipmentByOwner(string ownerId)
    {
        try
        {
            var equipment = await _equipmentRepository.GetByOwnerIdAsync(ownerId);
            var response = equipment.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting equipment by owner");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get my equipment
    /// </summary>
    [HttpGet("my-equipment")]
    public async Task<ActionResult<IEnumerable<EquipmentResponseDto>>> GetMyEquipment()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var equipment = await _equipmentRepository.GetByOwnerIdAsync(userId);
            var response = equipment.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting my equipment");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get equipment by type
    /// </summary>
    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<EquipmentResponseDto>>> GetEquipmentByType(EquipmentType type)
    {
        try
        {
            var equipment = await _equipmentRepository.GetByTypeAsync(type);
            var response = equipment.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting equipment by type");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get equipment by location
    /// </summary>
    [HttpGet("location/{locationId}")]
    public async Task<ActionResult<IEnumerable<EquipmentResponseDto>>> GetEquipmentByLocation(string locationId)
    {
        try
        {
            var equipment = await _equipmentRepository.GetByLocationIdAsync(locationId);
            var response = equipment.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting equipment by location");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get equipment available for rent
    /// </summary>
    [HttpGet("available-for-rent")]
    public async Task<ActionResult<IEnumerable<EquipmentResponseDto>>> GetAvailableForRent()
    {
        try
        {
            var equipment = await _equipmentRepository.GetAvailableForRentAsync();
            var response = equipment.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting equipment available for rent");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Get equipment available for sale
    /// </summary>
    [HttpGet("available-for-sale")]
    public async Task<ActionResult<IEnumerable<EquipmentResponseDto>>> GetAvailableForSale()
    {
        try
        {
            var equipment = await _equipmentRepository.GetAvailableForSaleAsync();
            var response = equipment.Select(MapToResponseDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting equipment available for sale");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Update equipment
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEquipment(string id, [FromBody] UpdateEquipmentDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var equipment = await _equipmentRepository.GetByIdAsync(id);
            if (equipment == null)
                return NotFound();

            if (equipment.OwnerId != userId)
                return Forbid("You can only update your own equipment");

            // Update fields if provided
            if (dto.Name != null) equipment.Name = dto.Name;
            if (dto.Type.HasValue) equipment.Type = dto.Type.Value;
            if (dto.Category.HasValue) equipment.Category = dto.Category;
            if (dto.Description != null) equipment.Description = dto.Description;
            if (dto.ImageUrl != null) equipment.ImageUrl = dto.ImageUrl;
            if (dto.Brand != null) equipment.Brand = dto.Brand;
            if (dto.Model != null) equipment.Model = dto.Model;
            if (dto.Sku != null) equipment.Sku = dto.Sku;
            if (dto.Quantity.HasValue) equipment.Quantity = dto.Quantity.Value;
            if (dto.Condition.HasValue) equipment.Condition = dto.Condition.Value;
            if (dto.PurchasePrice.HasValue) equipment.PurchasePrice = dto.PurchasePrice;
            if (dto.RentalPricePerDay.HasValue) equipment.RentalPricePerDay = dto.RentalPricePerDay;
            if (dto.Currency != null) equipment.Currency = dto.Currency;
            if (dto.LocationId != null) equipment.LocationId = dto.LocationId;
            if (dto.AvailableForRent.HasValue) equipment.AvailableForRent = dto.AvailableForRent.Value;
            if (dto.AvailableForSale.HasValue) equipment.AvailableForSale = dto.AvailableForSale.Value;
            if (dto.IsActive.HasValue) equipment.IsActive = dto.IsActive.Value;
            if (dto.MaintenanceDate.HasValue) equipment.MaintenanceDate = dto.MaintenanceDate;
            if (dto.PurchaseDate.HasValue) equipment.PurchaseDate = dto.PurchaseDate;
            if (dto.Tags != null) equipment.Tags = dto.Tags;
            if (dto.Specifications != null) equipment.Specifications = dto.Specifications;

            var updated = await _equipmentRepository.UpdateAsync(id, equipment);
            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating equipment");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Delete equipment
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEquipment(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var equipment = await _equipmentRepository.GetByIdAsync(id);
            if (equipment == null)
                return NotFound();

            if (equipment.OwnerId != userId)
                return Forbid("You can only delete your own equipment");

            var deleted = await _equipmentRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting equipment");
            return StatusCode(500, "An error occurred");
        }
    }

    private static EquipmentResponseDto MapToResponseDto(Equipment e) => new()
    {
        Id = e.Id!,
        Name = e.Name,
        Type = e.Type,
        Category = e.Category,
        Description = e.Description,
        ImageUrl = e.ImageUrl,
        Brand = e.Brand,
        Model = e.Model,
        Sku = e.Sku,
        Quantity = e.Quantity,
        Condition = e.Condition,
        PurchasePrice = e.PurchasePrice,
        RentalPricePerDay = e.RentalPricePerDay,
        Currency = e.Currency,
        OwnerId = e.OwnerId,
        LocationId = e.LocationId,
        AvailableForRent = e.AvailableForRent,
        AvailableForSale = e.AvailableForSale,
        IsActive = e.IsActive,
        MaintenanceDate = e.MaintenanceDate,
        PurchaseDate = e.PurchaseDate,
        Tags = e.Tags,
        Specifications = e.Specifications,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
