using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.IRespositories;
using Application.Dtos;
using AutoMapper;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BatteryHealthLogsService : IBatteryHealthlogsService
    {
        private readonly IBatteryHealthLogsRepository _batteryHealthLogsRepository;
        private readonly ILogger<BatteryHealthLogsService> _logger;
        private readonly IMapper _mapper;


        public BatteryHealthLogsService(IBatteryHealthLogsRepository batteryHealthLogsReposiroty, ILogger<BatteryHealthLogsService> logger, IMapper mapper)
        {
            _batteryHealthLogsRepository = batteryHealthLogsReposiroty;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(CreateBatteryHealthLogDto dto)
        {
            var battery = await _batteryHealthLogsRepository.GetBySerialNumberAsync(dto.SerialNumber);
            if (battery == null)
            {
                _logger.LogWarning("Battery not found for serial number: {Serial}", dto.SerialNumber);
                throw new KeyNotFoundException($"Battery with serial number {dto.SerialNumber} not found.");
            }

            var log = new BatteryHealthLog
            {
                BatteryId = battery.BatteryId,
                RecordedAt = dto.RecordedAt,
                SoH = dto.SoH,
                CycleCount = dto.CycleCount,
                Temperature = dto.Temperature,
                Notes = dto.Notes
            };

            await _batteryHealthLogsRepository.AddAsync(log);
            _logger.LogInformation("Added health log for battery {Serial}", dto.SerialNumber);
            return true;
        }

        public async Task<IEnumerable<BatteryHealthLogsDto>> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all Battery Health Logs...");

                var batteryHealthLogs = await _batteryHealthLogsRepository.GetAllBatteryHealthLogs();

                if (batteryHealthLogs == null)
                {
                    _logger.LogWarning("No battery health logs found in the database.");
                    return new List<BatteryHealthLogsDto>();
                }

                var dtos = _mapper.Map<IEnumerable<BatteryHealthLogsDto>>(batteryHealthLogs);

                _logger.LogInformation("Successfully retrieved {Count} battery health logs.", dtos.Count());

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching battery health logs.");
                throw; 
            }
        }

        public async Task<BatteryHealthLogsDto> UpdateAsync(int id, UpdateBatteryHealthLogDto dto)
        {
            var log = await _batteryHealthLogsRepository.GetByIdAsync(id);
            if (log == null)
            {
                throw new Exception($"BatteryHealthLog with ID {id} not found.");
            }

            log.RecordedAt = dto.RecordedAt;
            log.SoH = dto.SoH;
            log.CycleCount = dto.CycleCount;
            log.Temperature = dto.Temperature;
            log.Notes = dto.Notes;

            await _batteryHealthLogsRepository.UpdateAsync(log);

            return new BatteryHealthLogsDto
            {
                BatteryHealthLogId = log.BatteryHealthLogId,
                BatteryId = log.BatteryId,
                SerialNumber = log.Battery?.SerialNumber ?? "",
                RecordedAt = log.RecordedAt,
                SoH = log.SoH,
                CycleCount = log.CycleCount,
                Temperature = log.Temperature,
                Notes = log.Notes
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var log = await _batteryHealthLogsRepository.GetByIdAsync(id);
            if (log == null)
            {
                return false;
            }

            await _batteryHealthLogsRepository.DeleteAsync(log);
            return true;
        }
    }
}
