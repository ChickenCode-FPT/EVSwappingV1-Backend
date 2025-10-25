using Application.Dtos.Osrm;

namespace Application.Common.Interfaces.Services
{
    public interface IOSRMService
    {
        Task<OsrmTableResponse> GetTable(
            CoordinateDto start,
            IEnumerable<CoordinateDto> destinations,
            string profile = "car");

        Task<OsrmRouteResponse> GetRoute(
            CoordinateDto start,
            CoordinateDto end,
            string profile = "car");
    }
}
