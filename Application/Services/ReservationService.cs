using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Application.Common.Interfaces.Services;
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
        private readonly IUserRepository _userRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public ReservationService(
            IReservationRepository reservationRepo,
            IStationInventoryRepository inventoryRepo,
            IReservationAllocationRepository reservationAllocationRepo,
            IUserRepository userRepo,
            IVehicleRepository vehicleRepo,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _reservationRepo = reservationRepo;
            _inventoryRepo = inventoryRepo;
            _reservationAllocationRepo = reservationAllocationRepo;
            _userRepo = userRepo;
            _vehicleRepo = vehicleRepo;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<ReservationDto> CreateReservation(CreateReservationRequest request)
        {
            // ===== 1️⃣ Lấy user hiện tại =====
            var userId = _currentUser.UserId
                ?? throw new UnauthorizedAccessException("Không xác định được người dùng từ token.");

            var user = await _userRepo.GetByIdWithDetailsAsync(userId)
                ?? throw new InvalidOperationException("Không tìm thấy thông tin người dùng.");

            // ===== 2️⃣ Kiểm tra điều kiện bắt buộc =====
            if (user.Driver == null)
                throw new InvalidOperationException("Bạn cần đăng ký trở thành tài xế trước khi đặt lịch.");

            var userVehicles = user.Vehicles?.ToList() ?? new List<Vehicle>();
            if (!userVehicles.Any())
                throw new InvalidOperationException("Bạn cần thêm ít nhất một phương tiện trước khi đặt lịch.");

            // ===== 3️⃣ Lấy vehicle =====
            Vehicle vehicle;
            if (request.VehicleId.HasValue)
            {
                vehicle = userVehicles.FirstOrDefault(v => v.VehicleId == request.VehicleId.Value)
                    ?? throw new InvalidOperationException("Phương tiện không thuộc sở hữu của bạn.");
            }
            else
            {
                // nếu không chỉ định xe -> chọn xe đầu tiên (hoặc xe mặc định nếu có field IsDefault)
                vehicle = userVehicles.First();
            }

            if (vehicle.BatteryModelPreferenceId == null)
                throw new InvalidOperationException("Phương tiện của bạn chưa được gán model pin phù hợp.");

            var requiredModelId = vehicle.BatteryModelPreferenceId.Value;

            // ===== 4️⃣ Validate thời gian =====
            var fromUtc = request.ReservedFrom.ToUniversalTime();
            var toUtc = request.ReservedTo.ToUniversalTime();

            if (toUtc <= fromUtc)
                throw new InvalidOperationException("Thời gian đặt không hợp lệ.");

            if ((toUtc - fromUtc).TotalMinutes > 90)
                throw new InvalidOperationException("Thời lượng đặt tối đa là 90 phút.");

            // ===== 5️⃣ Kiểm tra lịch trùng =====
            var existing = await _reservationRepo.GetByUserId(userId);
            if (existing.Any(r =>
                    r.Status == ReservationStatus.Pending &&
                    r.ReservedFrom < toUtc &&
                    r.ReservedTo > fromUtc))
                throw new InvalidOperationException("Bạn đã có lịch đặt trùng thời gian.");

            // ===== 6️⃣ Kiểm tra pin khả dụng =====
            var available = await _inventoryRepo.CountAvailableBatteries(
                request.StationId, requiredModelId);

            if (available <= 0)
                throw new InvalidOperationException("Trạm hiện không còn pin đầy phù hợp với xe của bạn.");

            // ===== 7️⃣ Tìm pin phù hợp chưa bị giữ =====
            var candidateBatteries = await _inventoryRepo.GetFullBatteriesByModel(
                request.StationId, requiredModelId);

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
                throw new InvalidOperationException("Tất cả pin phù hợp đang được giữ. Vui lòng chọn khung giờ khác.");

            // ===== 8️⃣ Tạo Reservation =====
            var reservation = new Reservation
            {
                UserId = userId,
                StationId = request.StationId,
                VehicleId = vehicle.VehicleId,
                ReservedFrom = fromUtc,
                ReservedTo = toUtc,
                ReservedBatteryModelId = requiredModelId,
                Status = ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            await _reservationRepo.Add(reservation);

            // ===== 9️⃣ Giữ pin tương ứng =====
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

            // ===== 🔟 Trả về kết quả =====
            var reservationDto = _mapper.Map<ReservationDto>(reservation);
            reservationDto.Allocation = _mapper.Map<ReservationAllocationDto>(allocation);

            return reservationDto;
        }

        public async Task CancelReservation(CancelReservationRequest request)
        {
            var userId = _currentUser.UserId
                ?? throw new UnauthorizedAccessException("Không xác định được người dùng từ token.");

            var res = await _reservationRepo.GetById(request.ReservationId);
            if (res == null)
                throw new InvalidOperationException("Không tìm thấy đặt lịch.");

            if (res.UserId != userId)
                throw new UnauthorizedAccessException("Bạn không thể hủy đặt lịch này.");

            await _reservationRepo.Cancel(request.ReservationId);

            await _reservationAllocationRepo.ReleaseByReservation(
                request.ReservationId, ReservationAllocationStatus.Released);

            var allocations = await _reservationAllocationRepo.GetByReservationId(request.ReservationId);
            foreach (var alloc in allocations)
                await _inventoryRepo.MarkFull(alloc.BatteryId, res.StationId);
        }

        public async Task<IEnumerable<ReservationDto>> GetMyReservations()
        {
            var userId = _currentUser.UserId
                ?? throw new UnauthorizedAccessException("Không xác định được người dùng.");

            var reservations = await _reservationRepo.GetByUserId(userId);
            return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
        }
    }
}
