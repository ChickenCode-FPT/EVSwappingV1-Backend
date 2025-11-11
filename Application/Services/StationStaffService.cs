using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using AutoMapper;

namespace Application.Services
{
    public class StationStaffService : IStationStaffService
    {
        private readonly IStationStaffRepository _repository;
        private readonly IMapper _mapper;

        public StationStaffService(IStationStaffRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task AssignStaffAsync(int stationId, string userId, string role)
        {
            await _repository.AssignStaffAsync(stationId, userId, role);
        }

        public async Task<IEnumerable<StationStaffDto>> GetStationStaffsAsync(int stationId)
        {
            var stationStaffs = _mapper.Map<IEnumerable<StationStaffDto>>(await _repository.GetByStationIdAsync(stationId));
            return stationStaffs;
        }

        public async Task RemoveStaffAsync(int stationStaffId)
        {
            await _repository.RemoveStaffAsync(stationStaffId);
        }

        public async Task DeactivateStaffAsync(int stationStaffId)
        {
            await _repository.DeactivateStaffAsync(stationStaffId);
        }

        public async Task<IEnumerable<StationStaffDto>> GetStationStaffsByCodeAsync(string stationCode)
        {
            var staffs = await _repository.GetByStationCodeAsync(stationCode);
            return _mapper.Map<IEnumerable<StationStaffDto>>(staffs);
        }

        public async Task<IEnumerable<StationStaffDto>> GetStationStaffsByNameAsync(string stationName)
        {
            var staffs = await _repository.GetByStationNameAsync(stationName);
            return _mapper.Map<IEnumerable<StationStaffDto>>(staffs);
        }
    }
}
