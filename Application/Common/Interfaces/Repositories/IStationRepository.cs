using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IStationRepository
    {
        Task<Station?> GetById(int stationId);
        Task<IEnumerable<Station>> GetAll();
        Task<IEnumerable<Station>> GetNearbyStations(decimal latitude, decimal longitude, double radiusKm);
        Task Add(Station station);
        Task Update(Station station);
        Task Delete(int stationId);
    }
}
