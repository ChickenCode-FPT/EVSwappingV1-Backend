using Application.Batteries.Commands;
using Application.Dtos;
using AutoMapper;
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
            //battery
            CreateMap<Battery, BatteriesDto>();

            CreateMap<Driver, RegisterDriverResponse>();
            CreateMap<BatteryModel, BatteryModelDto>();

            CreateMap<RegisterSubscriptionRequest, Subscription>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.RemainingSwaps, opt => opt.Ignore());
            CreateMap<UpdateBatteryCommand, Battery>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));


            //swap transaction
            //CreateMap<UpdateSwapTransactionCommand, SwapTransaction>()
            //.ForMember(dest => dest.StaffUserId, opt => opt.MapFrom(src => src.StaffId))
            //.ForMember(dest => dest.CustomerUserId, opt => opt.MapFrom(src => src.CustomerId))
            //.ForMember(dest => dest.OutgoingBatteryId, opt => opt.MapFrom(src => src.OldBatteryId))
            //.ForMember(dest => dest.IncomingBatteryId, opt => opt.MapFrom(src => src.NewBatteryId))
            //.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Fee))
            //.ForMember(dest => dest.SwapStatus, opt => opt.MapFrom(src => src.SwapStatus));

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
            CreateMap<PaymentUpdateDto, Payment>()
           .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Battery, BatteryDto>();
            //station
            CreateMap<Station, StationInSwapDto>().ReverseMap();

            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.Allocation,
                    opt => opt.MapFrom(src => src.ReservationAllocations.FirstOrDefault()))
                .ReverseMap();

            CreateMap<ReservationAllocation, ReservationAllocationDto>().ReverseMap();

            CreateMap<Vehicle, VehicleDto>().ReverseMap();
            //reservation
            CreateMap<Reservation, ReverInSwapDto>().ReverseMap();

            CreateMap<RatingCreateRequest, Rating>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Rating, RatingDto>().ReverseMap();

            CreateMap<SupportTicket, SupportTicketDto>().ReverseMap();
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

        }
    }
}
