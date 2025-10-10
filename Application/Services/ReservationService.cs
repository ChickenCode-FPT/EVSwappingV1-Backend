using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using AutoMapper;
using Domain.Enums;
using Domain.Models;

namespace Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IStationInventoryRepository _inventoryRepo;
        private readonly IReservationAllocationRepository _reservationAllocationRepo;
        private readonly IMapper _mapper;

        public ReservationService(
            IReservationRepository reservationRepo,
            IStationInventoryRepository inventoryRepo,
            IReservationAllocationRepository reservationAllocationRepo,
            IMapper mapper)
        {
            _reservationRepo = reservationRepo;
            _inventoryRepo = inventoryRepo;
            _reservationAllocationRepo = reservationAllocationRepo;
            _mapper = mapper;
        }

        public async Task<ReservationDto> CreateReservation(CreateReservationRequest request)
        {
            var fromUtc = request.ReservedFrom.ToUniversalTime();
            var toUtc = request.ReservedTo.ToUniversalTime();

            if (string.IsNullOrEmpty(request.UserId))
                throw new InvalidOperationException("UserId không hợp lệ.");

            if (request.VehicleId == null)
                throw new InvalidOperationException("Thiếu thông tin xe.");

            if (toUtc <= fromUtc)
                throw new InvalidOperationException("Thời gian đặt không hợp lệ.");

            if ((toUtc - fromUtc).TotalMinutes > 90)
                throw new InvalidOperationException("Thời lượng đặt tối đa là 90 phút.");

            var existing = await _reservationRepo.GetByUserId(request.UserId);
            if (existing.Any(r => r.Status == ReservationStatus.Pending &&
                                  r.ReservedFrom < toUtc && r.ReservedTo > fromUtc))
                throw new InvalidOperationException("Bạn đã có lịch đặt trùng thời gian.");

            var available = await _inventoryRepo.CountAvailableBatteries(
                request.StationId, request.ReservedBatteryModelId);

            if (available <= 0)
                throw new InvalidOperationException("Trạm hiện không còn pin đầy cho model bạn chọn.");

            var candidateBatteries = await _inventoryRepo.GetFullBatteriesByModel(
                request.StationId, request.ReservedBatteryModelId);

            int? selectedBatteryId = null;
            foreach (var battery in candidateBatteries)
            {
                var activeHold = await _reservationAllocationRepo.GetActiveByBattery(
                    battery.BatteryId, fromUtc, toUtc);

                if (activeHold == null)
                {
                    selectedBatteryId = battery.BatteryId;
                    break;
                }
            }

            if (selectedBatteryId == null)
                throw new InvalidOperationException("Hiện tất cả pin đầy đã được giữ. Vui lòng chọn khung giờ khác.");

            var reservation = new Reservation
            {
                UserId = request.UserId,
                StationId = request.StationId,
                VehicleId = request.VehicleId,
                ReservedFrom = fromUtc,
                ReservedTo = toUtc,
                ReservedBatteryModelId = request.ReservedBatteryModelId,
                Status = ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            await _reservationRepo.Add(reservation);

            var allocation = new ReservationAllocation
            {
                ReservationId = reservation.ReservationId,
                BatteryId = selectedBatteryId.Value,
                AllocatedAt = DateTime.UtcNow,
                HoldUntil = fromUtc.AddMinutes(15),
                Status = ReservationAllocationStatus.Active
            };
            await _reservationAllocationRepo.Add(allocation);

            await _inventoryRepo.MarkHeld(selectedBatteryId.Value, request.StationId, reservation.ReservationId);

            var reservationDto = _mapper.Map<ReservationDto>(reservation);
            reservationDto.Allocation = _mapper.Map<ReservationAllocationDto>(allocation);

            return reservationDto;
        }

        public async Task CancelReservation(CancelReservationRequest request)
        {
            var res = await _reservationRepo.GetById(request.ReservationId);
            if (res == null || res.UserId != request.UserId)
                throw new UnauthorizedAccessException("Bạn ko thể hủy đặt lịch này.");

            await _reservationRepo.Cancel(request.ReservationId);

            await _reservationAllocationRepo.ReleaseByReservation(
                request.ReservationId, ReservationAllocationStatus.Released);

            var allocations = await _reservationAllocationRepo.GetByReservationId(request.ReservationId);
            foreach (var alloc in allocations)
            {
                await _inventoryRepo.MarkFull(alloc.BatteryId, res.StationId);
            }
        }

        public async Task<IEnumerable<ReservationDto>> GetReservationsByUser(string userId)
        {
            var reservations = await _reservationRepo.GetByUserId(userId);
            return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
        }
    }
}
