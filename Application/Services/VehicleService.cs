using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using Application.Dtos.Requests;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _repo;
        private readonly IMapper _mapper;

        public VehicleService(IVehicleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<VehicleDto?> GetById(int vehicleId)
        {
            var vehicle = await _repo.GetById(vehicleId);
            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<IEnumerable<VehicleDto>> GetByUser(string userId)
        {
            var vehicles = await _repo.GetByUserId(userId);
            return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
        }

        public async Task<VehicleDto> Create(CreateVehicleRequest request)
        {
            var vehicle = new Vehicle
            {
                UserId = request.UserId,
                Vin = request.Vin,
                Make = request.Make,
                Model = request.Model,
                Year = request.Year,
                BatteryModelPreferenceId = request.BatteryModelPreferenceId,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.Add(vehicle);
            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<VehicleDto> Update(UpdateVehicleRequest request)
        {
            var vehicle = await _repo.GetById(request.VehicleId);
            if (vehicle == null) throw new KeyNotFoundException("Vehicle not found");

            vehicle.Vin = request.Vin;
            vehicle.Make = request.Make;
            vehicle.Model = request.Model;
            vehicle.Year = request.Year;
            vehicle.BatteryModelPreferenceId = request.BatteryModelPreferenceId;

            await _repo.Update(vehicle);
            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task Delete(int vehicleId)
        {
            await _repo.Delete(vehicleId);
        }
    }
}
