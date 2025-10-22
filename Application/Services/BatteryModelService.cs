using Application.Common.Interfaces;
using Application.Common.IRespositories;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BatteryModelService : IBatteryModelService
    {
        private readonly IBatteryModelRepository _batteryModelRepository;

        public BatteryModelService(IBatteryModelRepository batteryModelRepository)
        {
            _batteryModelRepository = batteryModelRepository;
        }

        // Get all battery models
        public async Task<List<BatteryModel>> GetAll()
        {
            try
            {
                return await _batteryModelRepository.GetAll();
            }
            catch (Exception ex)
            {
                // Handle error, log, etc.
                throw new Exception("An error occurred while retrieving battery models.", ex);
            }
        }

        // Get a battery model by ID
        public async Task<BatteryModel?> GetById(int id)
        {
            try
            {
                return await _batteryModelRepository.GetById(id);
            }
            catch (Exception ex)
            {
                // Handle error, log, etc.
                throw new Exception("An error occurred while retrieving the battery model.", ex);
            }
        }

        // Add a new battery model
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
                // Handle error, log, etc.
                throw new Exception("An error occurred while adding the battery model.", ex);
            }
        }

        // Update an existing battery model
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
                // Handle error, log, etc.
                throw new Exception("An error occurred while updating the battery model.", ex);
            }
        }

        // Delete a battery model by ID
        public async Task Delete(int id)
        {
            try
            {
                await _batteryModelRepository.Delete(id);
            }
            catch (Exception ex)
            {
                // Handle error, log, etc.
                throw new Exception("An error occurred while deleting the battery model.", ex);
            }
        }
    }
}
