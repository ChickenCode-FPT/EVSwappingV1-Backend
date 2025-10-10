using Application.Dtos;
using Application.Dtos.Osrm;

namespace Application.Common.Interfaces.Services
{
    public interface IStationService
    {
        Task<StationDto?> GetById(int stationId);
        Task<IEnumerable<StationDto>> GetAll();
        Task<StationDto?> GetNearestStation(decimal userLng, decimal userLat);
        Task<OsrmRouteResponse> GetRouteToStation(decimal userLng, decimal userLat, int stationId);
    }
}
