using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Application.Common.Interfaces.Services;
using Application.Dtos.Payment;
using Application.Dtos.Reservation;
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
        private readonly ICurrentUserService _currentUser;
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public ReservationService(
            IReservationRepository reservationRepo,
            IStationInventoryRepository inventoryRepo,
            IReservationAllocationRepository reservationAllocationRepo,
            IUserRepository userRepo,
            ICurrentUserService currentUser,
            ISubscriptionRepository subscriptionRepo,
            IPaymentService paymentService,
            IMapper mapper)
        {
            _reservationRepo = reservationRepo;
            _inventoryRepo = inventoryRepo;
            _reservationAllocationRepo = reservationAllocationRepo;
            _userRepo = userRepo;
            _currentUser = currentUser;
            _subscriptionRepo = subscriptionRepo;
            _paymentService = paymentService;
            _mapper = mapper;
        }

        public async Task<ReservationDto> CreateReservation(CreateReservationRequest request)
        {
            var userId = _currentUser.UserId
                ?? throw new UnauthorizedAccessException("Không xác định được người dùng.");

            var user = await _userRepo.GetByIdWithDetailsAsync(userId)
                ?? throw new InvalidOperationException("Không tìm thấy thông tin người dùng.");

            if (user.Driver == null)
                throw new InvalidOperationException("Bạn cần đăng ký tài xế trước khi đặt lịch.");

            var vehicle = request.VehicleId.HasValue
                ? user.Vehicles.FirstOrDefault(v => v.VehicleId == request.VehicleId.Value)
                : user.Vehicles.FirstOrDefault();

            if (vehicle == null)
                throw new InvalidOperationException("Bạn chưa có phương tiện hợp lệ.");

            if (vehicle.BatteryModelPreferenceId == null)
                throw new InvalidOperationException("Phương tiện của bạn chưa được gán model pin phù hợp.");

            var modelId = vehicle.BatteryModelPreferenceId.Value;
            var fromUtc = request.ReservedFrom.ToUniversalTime();
            var toUtc = request.ReservedTo.ToUniversalTime();

            if (toUtc <= fromUtc)
                throw new InvalidOperationException("Thời gian đặt không hợp lệ.");

            if ((toUtc - fromUtc).TotalMinutes > 90)
                throw new InvalidOperationException("Thời lượng đặt tối đa là 90 phút.");

            if (fromUtc < DateTime.UtcNow.AddMinutes(10))
                throw new InvalidOperationException("Bạn chỉ được đặt trước ít nhất 10 phút.");

            var existing = await _reservationRepo.GetByUserId(userId);

            if (existing.Any(r => r.Status == ReservationStatus.Pending &&
                                  r.ReservedFrom < toUtc &&
                                  r.ReservedTo > fromUtc))
                throw new InvalidOperationException("Bạn đã có lịch trùng thời gian.");

            var sub = (await _subscriptionRepo.GetByUser(userId))
                .FirstOrDefault(s => s.Status == SubscriptionStatus.Active && s.EndDate >= DateTime.UtcNow);

            Reservation reservation;
            ReservationAllocation allocation;

            using (var transaction = await _reservationRepo.BeginTransactionAsync())
            {
                try
                {
                    var candidateIds = await _inventoryRepo.GetFullBatteryIdsByModel(request.StationId, modelId);

                    if (!candidateIds.Any())
                        throw new InvalidOperationException("Không còn pin đầy phù hợp.");

                    var overlappingIds = await _reservationAllocationRepo
                        .GetOverlappingBatteryIds(candidateIds, fromUtc, toUtc);

                    var freeBatteryId = candidateIds.Except(overlappingIds).FirstOrDefault();

                    if (freeBatteryId == 0)
                        throw new InvalidOperationException("Tất cả pin phù hợp đang được giữ. Vui lòng chọn khung giờ khác.");

                    var stillFree = await _reservationAllocationRepo
                        .IsBatteryFreeInWindow(freeBatteryId, fromUtc, toUtc);

                    if (!stillFree)
                        throw new InvalidOperationException("Pin vừa được giữ bởi người khác. Thử lại.");

                    reservation = new Reservation
                    {
                        UserId = userId,
                        StationId = request.StationId,
                        VehicleId = vehicle.VehicleId,
                        ReservedFrom = fromUtc,
                        ReservedTo = toUtc,
                        ReservedBatteryModelId = modelId,
                        Status = ReservationStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _reservationRepo.Add(reservation);

                    allocation = new ReservationAllocation
                    {
                        ReservationId = reservation.ReservationId,
                        BatteryId = freeBatteryId,
                        AllocatedAt = DateTime.UtcNow,
                        HoldUntil = fromUtc.AddMinutes(15),
                        Status = ReservationAllocationStatus.Active
                    };
                    await _reservationAllocationRepo.Add(allocation);

                    await transaction.CommitAsync();
                }
                catch
                {
                    throw;
                }
            }

            var dto = _mapper.Map<ReservationDto>(reservation);
            dto.Allocation = _mapper.Map<ReservationAllocationDto>(allocation);

            if (sub == null)
            {
                try
                {
                    var paymentDto = new PaymentCreateDto
                    {
                        UserId = userId,
                        ReservationId = reservation.ReservationId,
                        Type = PaymentType.ReservationDeposit,
                        Amount = 50000,
                        Currency = "VND",
                        Description = $"Deposit for reservation #{reservation.ReservationId}",
                        Method = "VNPAY"
                    };

                    var paymentResponse = await _paymentService.CreatePayment(paymentDto);
                    dto.PaymentCheckoutUrl = paymentResponse.CheckoutUrl;
                    dto.PaymentId = paymentResponse.PaymentId;
                    dto.PaymentStatus = paymentResponse.Status;
                }
                catch (Exception ex)
                {
                    reservation.Status = ReservationStatus.Cancelled;
                    await _reservationRepo.Update(reservation);
                    throw new InvalidOperationException($"Không thể tạo thanh toán đặt cọc: {ex.Message}", ex);
                }
            }
            else
            {
                reservation.Status = ReservationStatus.Confirmed;
                await _reservationRepo.Update(reservation);
                dto.Status = reservation.Status;
            }

            return dto;
        }

        public async Task CancelReservation(CancelReservationRequest request)
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException("Không xác định người dùng.");
            var res = await _reservationRepo.GetById(request.ReservationId)
                      ?? throw new InvalidOperationException("Không tìm thấy đặt lịch.");

            if (res.UserId != userId)
                throw new UnauthorizedAccessException("Bạn không thể hủy đặt lịch này.");

            using var tx = await _reservationRepo.BeginTransactionAsync();

            await _reservationRepo.Cancel(request.ReservationId);
            await _reservationAllocationRepo.ReleaseByReservation(request.ReservationId, "Cancelled");

            await tx.CommitAsync();
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
