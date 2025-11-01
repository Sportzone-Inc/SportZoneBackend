using SportZone.Models;

namespace SportZone.Repositories;

public interface IEquipmentRepository
{
    Task<Equipment> CreateAsync(Equipment equipment);
    Task<Equipment?> GetByIdAsync(string id);
    Task<IEnumerable<Equipment>> GetAllAsync();
    Task<IEnumerable<Equipment>> GetByOwnerIdAsync(string ownerId);
    Task<IEnumerable<Equipment>> GetByTypeAsync(EquipmentType type);
    Task<IEnumerable<Equipment>> GetByLocationIdAsync(string locationId);
    Task<IEnumerable<Equipment>> GetAvailableForRentAsync();
    Task<IEnumerable<Equipment>> GetAvailableForSaleAsync();
    Task<bool> UpdateAsync(string id, Equipment equipment);
    Task<bool> DeleteAsync(string id);
}
