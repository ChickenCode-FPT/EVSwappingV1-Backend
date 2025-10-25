using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Application.Common.Interfaces.Services;
using Application.Dtos;
using Application.Dtos.Requests;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IDriverRepository _driverRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public VehicleService(
            IVehicleRepository vehicleRepo,
            IDriverRepository driverRepo,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _vehicleRepo = vehicleRepo;
            _driverRepo = driverRepo;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<VehicleDto?> GetById(int vehicleId)
        {
            var vehicle = await _vehicleRepo.GetById(vehicleId);
            if (vehicle == null) return null;

            var userId = _currentUser.UserId;
            if (vehicle.UserId != userId)
                throw new UnauthorizedAccessException("Bạn không có quyền xem phương tiện này.");

            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<IEnumerable<VehicleDto>> GetByUser()
        {
            var userId = _currentUser.UserId
                ?? throw new UnauthorizedAccessException("Không xác định được người dùng.");
            var vehicles = await _vehicleRepo.GetByUserId(userId);
            return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
        }

        public async Task<VehicleDto> Create(CreateVehicleRequest request)
        {
            var userId = _currentUser.UserId
                ?? throw new UnauthorizedAccessException("Không xác định được người dùng.");

            var driver = await _driverRepo.GetByUserId(userId);
            if (driver == null)
                throw new InvalidOperationException("Bạn cần đăng ký làm tài xế trước khi tạo phương tiện.");

            var vehicle = new Vehicle
            {
                UserId = userId,
                Vin = request.Vin,
                Make = request.Make,
                Model = request.Model,
                Year = request.Year,
                BatteryModelPreferenceId = request.BatteryModelPreferenceId,
                CreatedAt = DateTime.UtcNow
            };

            await _vehicleRepo.Add(vehicle);
            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<VehicleDto> Update(UpdateVehicleRequest request)
        {
            var userId = _currentUser.UserId
                ?? throw new UnauthorizedAccessException("Không xác định được người dùng.");

            var vehicle = await _vehicleRepo.GetById(request.VehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException("Không tìm thấy phương tiện.");

            if (vehicle.UserId != userId)
                throw new UnauthorizedAccessException("Bạn không thể chỉnh sửa phương tiện này.");

            vehicle.Vin = request.Vin;
            vehicle.Make = request.Make;
            vehicle.Model = request.Model;
            vehicle.Year = request.Year;
            vehicle.BatteryModelPreferenceId = request.BatteryModelPreferenceId;

            await _vehicleRepo.Update(vehicle);
            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task Delete(int vehicleId)
        {
            var userId = _currentUser.UserId
                ?? throw new UnauthorizedAccessException("Không xác định được người dùng.");

            var vehicle = await _vehicleRepo.GetById(vehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException("Không tìm thấy phương tiện.");

            if (vehicle.UserId != userId)
                throw new UnauthorizedAccessException("Bạn không thể xoá phương tiện này.");

            await _vehicleRepo.Delete(vehicleId);
        }
    }
}
