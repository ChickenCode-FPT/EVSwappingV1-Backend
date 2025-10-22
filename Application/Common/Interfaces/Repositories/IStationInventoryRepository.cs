using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IStationInventoryRepository
    {
        Task<IEnumerable<Domain.Models.StationInventory>> GetByStationId(int stationId);
        Task<IEnumerable<Domain.Models.StationInventory>> GetAvailableBatteries(int stationId, int? batteryModelId = null);
        Task<Domain.Models.StationInventory?> GetById(int inventoryId);
        Task Add(Domain.Models.StationInventory inventory);
        Task Update(Domain.Models.StationInventory inventory);
        Task Delete(int inventoryId);

        Task<Domain.Models.StationInventory> GetInventory(int stationId, CancellationToken ct);
        Task<IEnumerable<Domain.Models.StationInventory>> GetInventorys(CancellationToken ct);
        //Task<int> GetBatteryCountByStatus(int stationId, string status);
    }
}
