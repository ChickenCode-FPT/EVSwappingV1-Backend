using Application.Batteries.Commands;
using Application.Dtos;
using Application.Dtos.Battery;
using Application.Dtos.Driver;
using Application.Dtos.Payment;
using Application.Dtos.Reservation;
using Application.Dtos.Station;
using Application.Dtos.Subscription;
using Application.Dtos.Swap;
using Application.Dtos.User;
using Application.SwapTransactions.Commands;
using AutoMapper;
using Domain.Dtos;
using Domain.Models;

namespace Application.Mappings
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<RegisterDriverRequest, Driver>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.TotalSwaps, opt => opt.MapFrom(_ => 0));

            CreateMap<Battery, BatteriesDto>();

            CreateMap<Driver, RegisterDriverResponse>();

            CreateMap<BatteryModel, BatteryModelDto>()
                .ForMember(dest => dest.DisplayName,
                    opt => opt.MapFrom(src => $"{src.Manufacturer} {src.ModelCode}"))
                .ForMember(dest => dest.CapacityWh,
                    opt => opt.MapFrom(src => (int)(src.CapacityKwh * 1000)));

            CreateMap<RegisterSubscriptionRequest, Subscription>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.RemainingSwaps, opt => opt.Ignore());
            CreateMap<UpdateBatteryCommand, Battery>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            //station inventory
            CreateMap<StationInventory, StationInventoryDto>()
                .ForMember(dest => dest.Batteries, opt => opt.MapFrom(src => src.Battery));

            //Battery
            CreateMap<CreateBatteryCommand, Battery>();
            CreateMap<UpdateBatteryStatusCommand, Battery>();



            //swap transaction
            CreateMap<UpdateSwapTransactionCommand, SwapTransaction>()
                .ForMember(dest => dest.StaffUserId, opt => opt.MapFrom(src => src.StaffId))
                .ForMember(dest => dest.CustomerUserId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Fee))
                .ForMember(dest => dest.SwapStatus, opt => opt.MapFrom(src => src.SwapStatus));


            CreateMap<TranscationDto, SwapTransaction>().ReverseMap();

            CreateMap<Subscription, RegisterSubscriptionResponse>();
            CreateMap<SwapTransaction, SwapTranscationFullDto>()
            .ForMember(dest => dest.Station, opt => opt.MapFrom(src => src.Station))
            .ForMember(dest => dest.Reservation, opt => opt.MapFrom(src => src.Reservation))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerUser.FullName))
            .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.StaffUser.FullName))
                .ReverseMap();

            CreateMap<CreatePackageRequest, SubscriptionPackage>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<SubscriptionPackage, SubscriptionPackageDto>();

            //payment
            CreateMap<Payment, PaymentAndTranDto>()
            .ForMember(dest => dest.Transcation, opt => opt.MapFrom(src => src.SwapTransaction))
                .ReverseMap();

            CreateMap<Station, StationDto>()
                .ForMember(dest => dest.AvailableBatteries, opt => opt.Ignore());

            CreateMap<Battery, BatteryDto>();

            //station
            CreateMap<Station, StationInSwapDto>().ReverseMap();

            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.Station,
                    opt => opt.MapFrom(src => src.Station ?? null))
                .ForMember(dest => dest.BatteryModel,
                    opt => opt.MapFrom(src =>
                        src.ReservationAllocations != null
                            ? src.ReservationAllocations
                                .Where(a => a.Battery != null && a.Battery.BatteryModel != null)
                                .Select(a => a.Battery.BatteryModel)
                                .FirstOrDefault()
                            : null))
                .ForMember(dest => dest.Allocation,
                    opt => opt.MapFrom(src =>
                        src.ReservationAllocations != null
                            ? src.ReservationAllocations.FirstOrDefault()
                            : null))
                .ForMember(dest => dest.PaymentCheckoutUrl,
                    opt => opt.MapFrom(src =>
                        src.Payments != null
                            ? src.Payments
                                .OrderByDescending(p => p.CreatedAt)
                                .Select(p => p.CheckoutUrl)
                                .FirstOrDefault()
                            : null))
                .ForMember(dest => dest.PaymentId,
                    opt => opt.MapFrom(src =>
                        src.Payments != null
                            ? src.Payments
                                .OrderByDescending(p => p.CreatedAt)
                                .Select(p => (long?)p.PaymentId)
                                .FirstOrDefault()
                            : null))
                .ForMember(dest => dest.PaymentStatus,
                    opt => opt.MapFrom(src =>
                        src.Payments != null
                            ? src.Payments
                                .OrderByDescending(p => p.CreatedAt)
                                .Select(p => p.Status)
                                .FirstOrDefault()
                            : null))
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(s => s.UpdatedAt));


            CreateMap<ReservationAllocation, ReservationAllocationDto>().ReverseMap();

            CreateMap<Vehicle, VehicleDto>().
                ForMember(dest => dest.BatteryModel, opt => opt.MapFrom(src => src.BatteryModelPreference))
                .ReverseMap();

            //reservation
            CreateMap<Reservation, ReverInSwapDto>().ReverseMap();

            CreateMap<RatingCreateRequest, Rating>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Rating, RatingDto>().ReverseMap();

            CreateMap<SupportTicket, SupportTicketDto>().ReverseMap();

            CreateMap<Payment, PaymentResponseDto>()
                .ForMember(dest => dest.GatewayOrderCode, opt => opt.MapFrom(src => src.PayOSOrderCode));

            CreateMap<Payment, PaymentSummaryDto>()
                .ForMember(dest => dest.RelatedEntity, opt => opt.Ignore());

            CreateMap<PaymentCreateDto, Payment>().ReverseMap();

            CreateMap<RefundRequestDto, Payment>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => Domain.Enums.PaymentType.Refund))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => Domain.Enums.PaymentStatus2.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<PenaltyPaymentDto, Payment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => Domain.Enums.PaymentStatus2.Pending))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Reason));

            CreateMap<Subscription, SubscriptionPaymentDto>()
                .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Package.Price))
                .ForMember(dest => dest.BillingCycle, opt => opt.MapFrom(src => src.Package.BillingCycle))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => $"Subscription for {src.Package.Name}"));

            CreateMap<SubscriptionPaymentDto, Payment>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => Domain.Enums.PaymentType.Subscription))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => Domain.Enums.PaymentStatus2.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.SubscriptionId, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionRef, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.TransactionRef)
                        ? $"SUB-{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}"
                        : src.TransactionRef));

            CreateMap<Subscription, SubscriptionDto>()
                .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Package.Price))
                .ForMember(dest => dest.BillingCycle, opt => opt.MapFrom(src => src.Package.BillingCycle))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.RemainingSwaps, opt => opt.MapFrom(src => src.RemainingSwaps));

            CreateMap<SubscriptionPackage, SubscriptionPackageDto>()
                .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.BillingCycle, opt => opt.MapFrom(src => src.BillingCycle))
                .ForMember(dest => dest.IncludedSwaps, opt => opt.MapFrom(src => src.IncludedSwaps))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<SwapTransaction, SwapTransactionDto2>().ReverseMap();
            CreateMap<CreateSwapTransactionRequest, SwapTransaction>();
            CreateMap<CompleteSwapTransactionRequest, SwapTransaction>();

            //batteryHealthLogs
            CreateMap<BatteryHealthLog, BatteryHealthLogsDto>()
                .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(src => src.Battery.SerialNumber));
            CreateMap<CreateBatteryHealthLogDto, BatteryHealthLog>()
               .ForMember(dest => dest.BatteryId, opt => opt.Ignore());

            //staionStaff
            CreateMap<StationStaff, StationStaffDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));

            //interStationTransfer
            CreateMap<InterStationTransfer, InterStationTransferDto>()
            .ForMember(dest => dest.FromStationName, opt => opt.MapFrom(src => src.FromStation.Name))
            .ForMember(dest => dest.ToStationName, opt => opt.MapFrom(src => src.ToStation.Name))
            .ForMember(dest => dest.BatteryCode, opt => opt.MapFrom(src => src.Battery.SerialNumber))
            .ForMember(dest => dest.RequestedByUserName, opt => opt.MapFrom(src => src.RequestedByUser.UserName))
            .ForMember(dest => dest.ApprovedByUserName, opt => opt.MapFrom(src => src.ApprovedByUser.UserName));

            //user
            CreateMap<User, StaffDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.Lockout, opt => opt.MapFrom(src => src.LockoutEnabled && src.LockoutEnd > DateTime.UtcNow));
            CreateMap<User, StaffUpdateDto>().ReverseMap();
        }
    }
}
