using Application.Dtos;

namespace Application.Common.Interfaces
{
    public interface IStationInventoryService
    {
        Task<StationInventoryDto> GetInventory(int stationId, CancellationToken ct);
        Task<IEnumerable<StationInventoryDto>> GetInventories(CancellationToken ct);
        //Task<int> GetBatteryCountByStatus(int stationId, string status);

    }
}
