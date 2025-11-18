using Application.Common.Interfaces;
using Application.Common.IRespositories;
using Application.Dtos.Battery;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class BatteryModelService : IBatteryModelService
    {
        private readonly IBatteryModelRepository _batteryModelRepository;
        private readonly IMapper _mapper;

        public BatteryModelService(IBatteryModelRepository batteryModelRepository, IMapper mapper)
        {
            _batteryModelRepository = batteryModelRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BatteryModelDto>> GetAll()
        {
            try
            {
                var batteryModels = await _batteryModelRepository.GetAll();
                return _mapper.Map<IEnumerable<BatteryModelDto>>(batteryModels);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving battery models.", ex);
            }
        }

        public async Task<BatteryModel?> GetById(int id)
        {
            try
            {
                return await _batteryModelRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the battery model.", ex);
            }
        }

        public async Task Add(BatteryModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Battery model cannot be null");
            }

            try
            {
                await _batteryModelRepository.Add(model);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the battery model.", ex);
            }
        }

        public async Task Update(BatteryModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Battery model cannot be null");
            }

            try
            {
                await _batteryModelRepository.Update(model);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the battery model.", ex);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await _batteryModelRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the battery model.", ex);
            }
        }
    }
}
