using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class InterStationTransferService : IInterStationTransferService
    {
        private readonly IInterStationTransferRepository _repository;
        private readonly IStationStaffRepository _stationStaffRepo;
        private readonly IStationInventoryRepository _stationInventoryRepository;

        private readonly IMapper _mapper;

        public InterStationTransferService(IInterStationTransferRepository repository, IMapper mapper, IStationStaffRepository stationStaffRepo, IStationInventoryRepository stationInventoryRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _stationStaffRepo = stationStaffRepo;
            _stationInventoryRepository = stationInventoryRepository;
        }

        public async Task<InterStationTransferDto> CreateTransferAsync(CreateTransferDto dto)
        {
            var transfer = new InterStationTransfer
            {
                FromStationId = dto.FromStationId,
                ToStationId = dto.ToStationId,
                BatteryId = dto.BatteryId,
                RequestedByUserId = dto.RequestedByUserId,
                RequestedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            await _repository.CreateAsync(transfer);
            var created = await _repository.GetByIdAsync(transfer.TransferId);
            return _mapper.Map<InterStationTransferDto>(created);
        }

        public async Task<bool> ApproveTransferAsync(long transferId, string approvedBy)
        {
            var transfer = await _repository.GetByIdAsync(transferId);
            if (transfer == null || transfer.Status != "Pending") return false;

            transfer.ApprovedByUserId = approvedBy;
            transfer.Status = "Approved";
            await _repository.UpdateAsync(transfer);
            return true;
        }

        public async Task<bool> CompleteTransferAsync(long transferId, CompleteInterStationTransfer completeInterStationTransfer)
        {
            var transfer = await _repository.GetByIdAsync(transferId);

            if (transfer == null || transfer.Status != "Approved") return false;

            try
            {
                await _repository.CompletedTransferAsync(transfer, completeInterStationTransfer);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        public async Task<IEnumerable<InterStationTransferDto>> GetTransfersByStationAsync(int stationId)
        {
            var transfers = await _repository.GetByStationAsync(stationId);
            return _mapper.Map<IEnumerable<InterStationTransferDto>>(transfers);
        }

        public async Task<List<GetInterStationTransferDto>> GetOutgoingTransfersAsync(string userId)
        {
            var staff = await _stationStaffRepo.GetActiveStaffByUserIdAsync(userId)
                        ?? throw new UnauthorizedAccessException("Staff chưa được gán vào trạm.");

            var transfers = await _repository.GetOutgoingTransfersAsync(staff.StationId);

            return transfers.Select(t => new GetInterStationTransferDto
            {
                TransferId = t.TransferId,
                FromStationId = t.FromStationId,
                FromStationName = t.FromStation.Name,
                ToStationId = t.ToStationId,
                ToStationName = t.ToStation.Name,
                BatteryId = t.BatteryId,
                BatterySerial = t.Battery.SerialNumber,
                Status = t.Status,
                RequestedAt = t.RequestedAt,
                CompletedAt = t.CompletedAt,
                RequestedBy = t.RequestedByUser.FullName,
                ApprovedBy = t.ApprovedByUser?.FullName
            }).ToList();
        }

        public async Task<List<GetInterStationTransferDto>> GetIncomingTransfersAsync(string userId)
        {
            var staff = await _stationStaffRepo.GetActiveStaffByUserIdAsync(userId)
                        ?? throw new UnauthorizedAccessException("Staff chưa được gán vào trạm.");

            var transfers = await _repository.GetIncomingTransfersAsync(staff.StationId);

            return transfers.Select(t => new GetInterStationTransferDto
            {
                TransferId = t.TransferId,
                FromStationId = t.FromStationId,
                FromStationName = t.FromStation.Name,
                ToStationId = t.ToStationId,
                ToStationName = t.ToStation.Name,
                BatteryId = t.BatteryId,
                BatterySerial = t.Battery.SerialNumber,
                Status = t.Status,
                RequestedAt = t.RequestedAt,
                CompletedAt = t.CompletedAt,
                RequestedBy = t.RequestedByUser.FullName,
                ApprovedBy = t.ApprovedByUser?.FullName
            }).ToList();
        }

        public async Task<List<InterStationTransferAdminDto>> GetAllTransfersAsync()
        {
            var entities = await _repository.GetAllTransfersAsync();
            var dtos = entities.Select(t => new InterStationTransferAdminDto
            {
                TransferId = t.TransferId,
                FromStationId = t.FromStationId,
                FromStationName = t.FromStation?.Name,
                ToStationId = t.ToStationId,
                ToStationName = t.ToStation?.Name,
                BatteryId = t.BatteryId,
                BatterySerial = t.Battery?.SerialNumber,
                RequestedByUserId = t.RequestedByUserId, // Map ID
                RequestedBy = t.RequestedByUser?.FullName, // Map Name
                ApprovedByUserId = t.ApprovedByUserId, // Map ID
                ApprovedBy = t.ApprovedByUser?.FullName, // Map Name
                Status = t.Status,
                RequestedAt = t.RequestedAt,
                CompletedAt = t.CompletedAt
            }).ToList();

            return dtos;
        }

        public async Task<List<String>> GetAvaiableSlot(int stationId)
        {
            var slots = await _repository.GetEmptySlotsAsync(stationId);
            if (slots == null || slots.Count == 0)
            {
                return new List<string>();
            }
            return slots;
        }

    }
}

