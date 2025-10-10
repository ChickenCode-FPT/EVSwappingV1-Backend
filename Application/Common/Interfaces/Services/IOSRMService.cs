
using Application.Dtos.Osrm;

namespace Application.Common.Interfaces.Services
{
    public interface IOSRMService
    {
        Task<OsrmTableResponse> GetTable((decimal lng, decimal lat) start, IEnumerable<(decimal lng, decimal lat)> stations);
        Task<OsrmRouteResponse> GetRoute((decimal lng, decimal lat) start, (decimal lng, decimal lat) end);
    }
}
