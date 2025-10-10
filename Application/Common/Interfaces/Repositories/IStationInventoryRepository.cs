using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IStationInventoryRepository
    {
        Task<IEnumerable<StationInventory>> GetByStationId(int stationId);

        Task<IEnumerable<StationInventory>> GetAvailableBatteries(int stationId, int? batteryModelId = null);

        Task<int> CountAvailableBatteries(int stationId, int? batteryModelId = null);

        Task<int> CountFullBatteries(int stationId, int batteryModelId);

        Task<StationInventory?> GetById(int inventoryId);

        Task Add(StationInventory inventory);
        Task Update(StationInventory inventory);
        Task Delete(int inventoryId);

        Task<IEnumerable<Battery>> GetFullBatteriesByModel(int stationId, int batteryModelId);

        Task MarkHeld(int batteryId, int stationId, int? reservationId = null);

        Task MarkFull(int batteryId, int stationId);
    }
}
