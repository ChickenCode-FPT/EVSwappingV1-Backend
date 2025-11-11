using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IStationStaffRepository
    {
        Task<IEnumerable<StationStaff>> GetAllAsync();
        Task<IEnumerable<StationStaff>> GetByStationIdAsync(int stationId);
        Task<IEnumerable<StationStaff>> GetByUserIdAsync(string userId);
        Task<StationStaff?> GetByIdAsync(int stationStaffId);
        Task AssignStaffAsync(int stationId, string userId, string role);
        Task DeactivateStaffAsync(int stationStaffId);
        Task RemoveStaffAsync(int stationStaffId);
        Task<IEnumerable<StationStaff>> GetByStationCodeAsync(string stationCode);
        Task<IEnumerable<StationStaff>> GetByStationNameAsync(string stationName);
        Task<StationStaff?> GetActiveStaffByUserIdAsync(string userId);
        Task<List<StationStaff>> GetStaffByStationIdAsync(int stationId);

    }
}
