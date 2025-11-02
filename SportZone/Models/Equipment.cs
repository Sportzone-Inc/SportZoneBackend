using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportZone.Models;

/// <summary>
/// Equipment entity for sports activities
/// </summary>
public class Equipment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    [BsonRequired]
    public string Name { get; set; } = string.Empty;

    [BsonElement("type")]
    [BsonRepresentation(BsonType.String)]
    [BsonRequired]
    public EquipmentType Type { get; set; }

    [BsonElement("category")]
    [BsonRepresentation(BsonType.String)]
    public EquipmentCategory? Category { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("imageUrl")]
    public string? ImageUrl { get; set; }

    [BsonElement("brand")]
    public string? Brand { get; set; }

    [BsonElement("model")]
    public string? Model { get; set; }

    [BsonElement("sku")]
    public string? Sku { get; set; }

    [BsonElement("quantity")]
    public int Quantity { get; set; } = 1;

    [BsonElement("condition")]
    [BsonRepresentation(BsonType.String)]
    public EquipmentCondition Condition { get; set; } = EquipmentCondition.New;

    [BsonElement("purchasePrice")]
    public decimal? PurchasePrice { get; set; }

    [BsonElement("rentalPricePerDay")]
    public decimal? RentalPricePerDay { get; set; }

    [BsonElement("currency")]
    public string Currency { get; set; } = "USD";

    [BsonElement("ownerId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? OwnerId { get; set; }

    [BsonElement("locationId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? LocationId { get; set; }

    [BsonElement("availableForRent")]
    public bool AvailableForRent { get; set; } = false;

    [BsonElement("availableForSale")]
    public bool AvailableForSale { get; set; } = false;

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("maintenanceDate")]
    public DateTime? MaintenanceDate { get; set; }

    [BsonElement("purchaseDate")]
    public DateTime? PurchaseDate { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    [BsonElement("specifications")]
    public Dictionary<string, string> Specifications { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Equipment type enumeration
/// </summary>
public enum EquipmentType
{
    Ball,
    Racket,
    Bat,
    Glove,
    Shoes,
    Clothing,
    ProtectiveGear,
    Net,
    Goal,
    Mat,
    Weights,
    CardioEquipment,
    TrainingAid,
    Accessories,
    Other
}

/// <summary>
/// Equipment category enumeration
/// </summary>
public enum EquipmentCategory
{
    Sports,
    Fitness,
    Training,
    Safety,
    Accessories,
    Maintenance,
    Other
}

/// <summary>
/// Equipment condition enumeration
/// </summary>
public enum EquipmentCondition
{
    New,
    Excellent,
    Good,
    Fair,
    Poor,
    NeedsRepair
}
