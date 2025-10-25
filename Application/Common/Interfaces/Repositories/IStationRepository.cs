using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IStationRepository
    {
        Task<Station?> GetById(int stationId);
        Task<IEnumerable<Station>> GetAll();
        Task Add(Station station);
        Task Update(Station station);
        Task Delete(int stationId);
    }
}
