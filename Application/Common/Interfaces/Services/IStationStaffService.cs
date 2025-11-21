using Application.Dtos;

namespace Application.Common.Interfaces.Services
{
    public interface IStationStaffService
    {
        Task AssignStaffAsync(int stationId, string userId, string role);
        Task<IEnumerable<StationStaffDto>> GetStationStaffsAsync(int stationId);
        Task<StationStaffDto?> GetByStaffId(string stationStaffId);
        Task RemoveStaffAsync(int stationStaffId);
        Task DeactivateStaffAsync(int stationStaffId);
        Task<IEnumerable<StationStaffDto>> GetStationStaffsByCodeAsync(string stationCode);
        Task<IEnumerable<StationStaffDto>> GetStationStaffsByNameAsync(string stationName);
    }
}
