using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using Microsoft.Extensions.Options;

namespace SportZone.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly IMongoCollection<Equipment> _equipment;

    public EquipmentRepository(IMongoDatabase database)
    {
        _equipment = database.GetCollection<Equipment>("equipment");

        // Create indexes
        _equipment.Indexes.CreateOneAsync(
            new CreateIndexModel<Equipment>(Builders<Equipment>.IndexKeys.Ascending(e => e.Type)));
        _equipment.Indexes.CreateOneAsync(
            new CreateIndexModel<Equipment>(Builders<Equipment>.IndexKeys.Ascending(e => e.OwnerId)));
        _equipment.Indexes.CreateOneAsync(
            new CreateIndexModel<Equipment>(Builders<Equipment>.IndexKeys.Ascending(e => e.LocationId)));
        _equipment.Indexes.CreateOneAsync(
            new CreateIndexModel<Equipment>(Builders<Equipment>.IndexKeys.Ascending(e => e.AvailableForRent)));
        _equipment.Indexes.CreateOneAsync(
            new CreateIndexModel<Equipment>(Builders<Equipment>.IndexKeys.Ascending(e => e.AvailableForSale)));
    }

    public async Task<Equipment> CreateAsync(Equipment equipment)
    {
        equipment.CreatedAt = DateTime.UtcNow;
        equipment.UpdatedAt = DateTime.UtcNow;
        await _equipment.InsertOneAsync(equipment);
        return equipment;
    }

    public async Task<Equipment?> GetByIdAsync(string id)
    {
        return await _equipment.Find(e => e.Id == id && e.IsActive).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync()
    {
        return await _equipment.Find(e => e.IsActive)
            .SortByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Equipment>> GetByOwnerIdAsync(string ownerId)
    {
        return await _equipment.Find(e => e.OwnerId == ownerId && e.IsActive)
            .SortByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Equipment>> GetByTypeAsync(EquipmentType type)
    {
        return await _equipment.Find(e => e.Type == type && e.IsActive)
            .SortByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Equipment>> GetByLocationIdAsync(string locationId)
    {
        return await _equipment.Find(e => e.LocationId == locationId && e.IsActive)
            .SortByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Equipment>> GetAvailableForRentAsync()
    {
        return await _equipment.Find(e => e.AvailableForRent && e.IsActive)
            .SortByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Equipment>> GetAvailableForSaleAsync()
    {
        return await _equipment.Find(e => e.AvailableForSale && e.IsActive)
            .SortByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(string id, Equipment equipment)
    {
        equipment.UpdatedAt = DateTime.UtcNow;
        var result = await _equipment.ReplaceOneAsync(e => e.Id == id, equipment);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var update = Builders<Equipment>.Update.Set(e => e.IsActive, false);
        var result = await _equipment.UpdateOneAsync(e => e.Id == id, update);
        return result.ModifiedCount > 0;
    }
}
