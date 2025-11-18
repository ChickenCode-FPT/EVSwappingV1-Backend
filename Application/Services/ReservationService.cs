using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Application.Common.Interfaces.Services;
using Application.Common.IRespositories;
using Application.Dtos.Payment;
using Application.Dtos.Reservation;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IStationInventoryRepository _inventoryRepo;
        private readonly IReservationAllocationRepository _reservationAllocationRepo;
        private readonly IUserRepository _userRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IPaymentService _paymentService;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IBatteryModelRepository _batteryModelRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(
            IReservationRepository reservationRepo,
            IStationInventoryRepository inventoryRepo,
            IReservationAllocationRepository reservationAllocationRepo,
            IUserRepository userRepo,
            ICurrentUserService currentUser,
            IPaymentService paymentService,
            IVehicleRepository vehicleRepo,
            IBatteryModelRepository batteryModelRepo,
            IMapper mapper,
            ILogger<ReservationService> logger)
        {
            _reservationRepo = reservationRepo;
            _inventoryRepo = inventoryRepo;
            _reservationAllocationRepo = reservationAllocationRepo;
            _userRepo = userRepo;
            _currentUser = currentUser;
            _paymentService = paymentService;
            _vehicleRepo = vehicleRepo;
            _batteryModelRepo = batteryModelRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReservationDto> CreateReservation(CreateReservationRequest request)
        {
            var userId = _currentUser.UserId;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("Không xác định người dùng.");
            }

            var user = await _userRepo.GetByIdWithDetailsAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin người dùng.");
            }


            if (user.Driver == null)
            {
                throw new InvalidOperationException("Bạn cần đăng ký tài xế trước khi đặt lịch.");
            }

            if (!request.VehicleId.HasValue)
            {
                throw new InvalidOperationException("Chưa chọn phương tiện hợp lệ.");
            }

            if (!await _vehicleRepo.IsValidVehicleByUser(userId, request.VehicleId.Value))
            {
                throw new InvalidOperationException("Phương tiện không hợp lệ.");
            }

            var vehicle = await _vehicleRepo.GetById(request.VehicleId.Value);
            if (vehicle.BatteryModelPreferenceId == null)
            {
                throw new InvalidOperationException("Xe chưa gán model pin phù hợp.");
            }

            var fromUtc = request.ReservedFrom.ToUniversalTime();
            var toUtc = request.ReservedTo.ToUniversalTime();

            if (toUtc <= fromUtc)
            {
                throw new InvalidOperationException("Thời gian đặt không hợp lệ.");
            }

            if ((toUtc - fromUtc).TotalMinutes > 90)
            {
                throw new InvalidOperationException("Thời lượng đặt tối đa 90 phút.");
            }

            if (fromUtc < DateTime.UtcNow.AddMinutes(10))
            {
                throw new InvalidOperationException("Phải đặt trước ít nhất 10 phút.");
            }

            var userRese = await _reservationRepo.GetByUserId(userId);
            if (userRese.Any(r => r.Status == ReservationStatus.Pending && r.ReservedFrom < toUtc && r.ReservedTo > fromUtc))
            {
                throw new InvalidOperationException("Bạn đã có lịch trùng thời gian.");
            }

            var batteryModel = await _batteryModelRepo.GetById(vehicle.BatteryModelPreferenceId.Value);
            if (batteryModel == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin model pin.");
            }

            Reservation reservation;
            ReservationAllocation allocation;

            using (var transaction = await _reservationRepo.BeginTransactionAsync())
            {
                var candidateIds = await _inventoryRepo.GetFullBatteryIdsByModel(request.StationId, vehicle.BatteryModelPreferenceId.Value);
                if (!candidateIds.Any())
                {
                    throw new InvalidOperationException("Không còn pin đầy phù hợp.");
                }

                var overlapping = await _reservationAllocationRepo.GetOverlappingBatteryIds(candidateIds, fromUtc, toUtc);
                var freeBatteryId = candidateIds.Except(overlapping).FirstOrDefault();
                if (freeBatteryId == 0)
                {
                    throw new InvalidOperationException("Tất cả pin phù hợp đang được giữ.");
                }

                reservation = new Reservation
                {
                    UserId = userId,
                    StationId = request.StationId,
                    VehicleId = vehicle.VehicleId,
                    ReservedFrom = fromUtc,
                    ReservedTo = toUtc,
                    ReservedBatteryModelId = vehicle.BatteryModelPreferenceId.Value,
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

            var dto = _mapper.Map<ReservationDto>(reservation);
            dto.Allocation = _mapper.Map<ReservationAllocationDto>(allocation);

            try
            {
                var paymentDto = new PaymentCreateDto
                {
                    UserId = userId,
                    ReservationId = reservation.ReservationId,
                    Type = PaymentType.ReservationDeposit,
                    Amount = batteryModel.ReservationDepositFee,
                    Currency = "VND",
                    Description = $"Deposit for reservation #{reservation.ReservationId}",
                    Method = "VNPAY"
                };

                var paymentResp = await _paymentService.CreatePayment(paymentDto);
                dto.PaymentCheckoutUrl = paymentResp.CheckoutUrl;
                dto.PaymentId = paymentResp.PaymentId;
                dto.PaymentStatus = paymentResp.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể tạo thanh toán đặt cọc.");
                throw new InvalidOperationException("Lỗi tạo thanh toán đặt cọc, vui lòng thử lại.");
            }

            return dto;
        }

        //public async Task<ReservationDto> CreateReservation(CreateReservationRequest request)
        //{
        //    var userId = _currentUser.UserId;
        //    if(userId == null)
        //    {
        //        throw new UnauthorizedAccessException("Ko xác định user");
        //    }

        //    var user = await _userRepo.GetByIdWithDetailsAsync(userId);
        //    if(user == null)
        //    {
        //        throw new UnauthorizedAccessException("Ko xác định user");
        //    }

        //    if(user.Driver == null)
        //    {
        //        throw new InvalidOperationException("Ban can đang ky Driver trươc");
        //    }

        //    if (!request.VehicleId.HasValue)
        //    {
        //        throw new InvalidOperationException("Ban chua chon phuong tien hop le");
        //    }

        //    if(!await _vehicleRepo.IsValidVehicleByUser(userId, request.VehicleId.Value))
        //    {
        //        throw new InvalidOperationException("Vehicale ko hop le");
        //    }

        //    var vehicle = await _vehicleRepo.GetById(request.VehicleId.Value);
        //    if (vehicle.BatteryModelPreference == null) 
        //    {
        //        throw new InvalidOperationException("Ban chua dk modle phu hop cho phuong tien cua ban");
        //    }

        //    var batteryModel = await _batteryModelRepo.GetById(vehicle.BatteryModelPreferenceId.Value);
        //    if(batteryModel == null)
        //    {
        //        throw new InvalidOperationException("Ko co model hop le");
        //    }

        //    var fromUtc = request.ReservedFrom.ToUniversalTime();
        //    var toUtc = request.ReservedTo.ToUniversalTime();

        //    if (fromUtc >= toUtc)
        //    {
        //        throw new InvalidOperationException("Thoi gian dat ko hop le");
        //    }

        //    if((toUtc - fromUtc).TotalMinutes > 90)
        //    {
        //        throw new InvalidOperationException("Thoi luong dat ko qua 90p");
        //    }

        //    if(DateTime.UtcNow.AddMinutes(10) > fromUtc)
        //    {
        //        throw new InvalidOperationException("Phai dat truoc it nhat 10p");
        //    }

        //    if(!await _reservationRepo.HasOverlappingReservation(userId, fromUtc, toUtc))
        //    {
        //        throw new InvalidOperationException("Ban da co lich trung truoc do");
        //    }

        //    Reservation reservation;
        //    ReservationAllocation allocation;

        //    using (var transaction = await _reservationRepo.BeginTransactionAsync())
        //    {

        //    }
        //}

        public async Task CancelReservation(CancelReservationRequest request)
        {
            var userId = _currentUser.UserId;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("Không xác định người dùng.");
            }

            var res = await _reservationRepo.GetById(request.ReservationId);
            if(res == null)
            {
                throw new InvalidOperationException("Không tìm thấy đặt lịch.");
            }

            if (res.UserId != userId)
            {
                throw new UnauthorizedAccessException("Bạn không thể hủy đặt lịch này.");
            }

            using var tx = await _reservationRepo.BeginTransactionAsync();

            await _reservationRepo.Cancel(request.ReservationId);
            await _reservationAllocationRepo.ReleaseByReservation(request.ReservationId, ReservationStatus.Cancelled);

            await tx.CommitAsync();
        }

        public async Task<IEnumerable<ReservationDto>> GetMyReservations()
        {
            var userId = _currentUser.UserId;

            if(userId == null)
            {
                throw new UnauthorizedAccessException("Không xác định người dùng.");
            }

            var reservations = await _reservationRepo.GetByUserId(userId);
            return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
        }
    }
}
