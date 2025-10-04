using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IStationInventoryRepository
    {
        Task<IEnumerable<StationInventory>> GetByStationId(int stationId);
        Task<IEnumerable<StationInventory>> GetAvailableBatteries(int stationId, int? batteryModelId = null);
        Task<StationInventory?> GetById(int inventoryId);
        Task Add(StationInventory inventory);
        Task Update(StationInventory inventory);
        Task Delete(int inventoryId);
    }
}
