using Application.Dtos;

namespace Application.Common.Interfaces.Services
{
    public interface IBatteryService
    {
        Task<IEnumerable<BatteryDto>> GetAvailableBatteries(int stationId, int? batteryModelId = null);
    }
}
