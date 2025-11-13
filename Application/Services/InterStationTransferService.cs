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

        private readonly IMapper _mapper;

        public InterStationTransferService(IInterStationTransferRepository repository, IMapper mapper, IStationStaffRepository stationStaffRepo)
        {
            _repository = repository;
            _mapper = mapper;
            _stationStaffRepo = stationStaffRepo;
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

        public async Task<bool> CompleteTransferAsync(long transferId)
        {
            var transfer = await _repository.GetByIdAsync(transferId);
            if (transfer == null || transfer.Status != "Approved") return false;

            transfer.Status = "Completed";
            transfer.CompletedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(transfer);
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


    }
}
