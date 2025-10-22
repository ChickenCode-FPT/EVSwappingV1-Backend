using Application.Dtos;

namespace Application.Common.Interfaces.Services
{
    public interface IStationService
    {
        Task<IEnumerable<StationDto>> GetNearbyStations(decimal latitude, decimal longitude, double radiusKm);
        Task<StationDto?> GetById(int stationId);
    }
}
