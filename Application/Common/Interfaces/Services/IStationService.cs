using Application.Dtos;
using Application.Dtos.Osrm;

namespace Application.Common.Interfaces.Services
{
    public interface IStationService
    {
        Task<StationDto?> GetById(int stationId);
        Task<IEnumerable<StationDto>> GetAll();
        Task<StationDto?> GetNearestStation(decimal userLng, decimal userLat, string profile = "car");
        Task<OsrmRouteResponse> GetRouteToStation(decimal userLng, decimal userLat, int stationId, string profile = "car");
        Task<IEnumerable<StationDto>> GetAllWithDistanceAsync(decimal userLng, decimal userLat);
    }
}
