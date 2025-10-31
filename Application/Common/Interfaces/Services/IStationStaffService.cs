using Application.Common.Interfaces.Repositories;
using Application.Dtos;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Services
{
    public interface IStationStaffService
    {
        Task AssignStaffAsync(int stationId, string userId, string role);
        Task<IEnumerable<StationStaffDto>> GetStationStaffsAsync(int stationId);
        Task RemoveStaffAsync(int stationStaffId);
        Task DeactivateStaffAsync(int stationStaffId);
        Task<IEnumerable<StationStaffDto>> GetStationStaffsByCodeAsync(string stationCode);
        Task<IEnumerable<StationStaffDto>> GetStationStaffsByNameAsync(string stationName);
    }
}
