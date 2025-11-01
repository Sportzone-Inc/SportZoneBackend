using SportZone.Models;
using System.ComponentModel.DataAnnotations;

namespace SportZone.DTOs;

/// <summary>
/// DTO for creating equipment
/// </summary>
public class CreateEquipmentDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Type is required")]
    public EquipmentType Type { get; set; }

    public EquipmentCategory? Category { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
    public string? Brand { get; set; }

    [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
    public string? Model { get; set; }

    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    public string? Sku { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
    public int Quantity { get; set; } = 1;

    public EquipmentCondition Condition { get; set; } = EquipmentCondition.New;

    [Range(0, double.MaxValue, ErrorMessage = "Purchase price must be a positive number")]
    public decimal? PurchasePrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Rental price must be a positive number")]
    public decimal? RentalPricePerDay { get; set; }

    [StringLength(3, ErrorMessage = "Currency must be a 3-letter code")]
    public string Currency { get; set; } = "USD";

    public string? LocationId { get; set; }

    public bool AvailableForRent { get; set; } = false;

    public bool AvailableForSale { get; set; } = false;

    public DateTime? MaintenanceDate { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public List<string>? Tags { get; set; }

    public Dictionary<string, string>? Specifications { get; set; }
}

/// <summary>
/// DTO for updating equipment
/// </summary>
public class UpdateEquipmentDto
{
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string? Name { get; set; }

    public EquipmentType? Type { get; set; }

    public EquipmentCategory? Category { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
    public string? Brand { get; set; }

    [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
    public string? Model { get; set; }

    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    public string? Sku { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
    public int? Quantity { get; set; }

    public EquipmentCondition? Condition { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Purchase price must be a positive number")]
    public decimal? PurchasePrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Rental price must be a positive number")]
    public decimal? RentalPricePerDay { get; set; }

    [StringLength(3, ErrorMessage = "Currency must be a 3-letter code")]
    public string? Currency { get; set; }

    public string? LocationId { get; set; }

    public bool? AvailableForRent { get; set; }

    public bool? AvailableForSale { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? MaintenanceDate { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public List<string>? Tags { get; set; }

    public Dictionary<string, string>? Specifications { get; set; }
}

/// <summary>
/// DTO for equipment response
/// </summary>
public class EquipmentResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public EquipmentCategory? Category { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Sku { get; set; }
    public int Quantity { get; set; }
    public EquipmentCondition Condition { get; set; }
    public decimal? PurchasePrice { get; set; }
    public decimal? RentalPricePerDay { get; set; }
    public string Currency { get; set; } = "USD";
    public string? OwnerId { get; set; }
    public string? LocationId { get; set; }
    public bool AvailableForRent { get; set; }
    public bool AvailableForSale { get; set; }
    public bool IsActive { get; set; }
    public DateTime? MaintenanceDate { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, string> Specifications { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
