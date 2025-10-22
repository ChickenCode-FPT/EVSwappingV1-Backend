using Application.Batteries.Commands;
using Application.Dtos;
using Application.SwapTransactions.Commands;
using AutoMapper;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Mappings
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            //battery
            CreateMap<Battery, BatteriesDto>();

            CreateMap<BatteryModel, BatteryModelDto>();

            CreateMap<UpdateBatteryCommand, Battery>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));


            //swap transaction
            CreateMap<UpdateSwapTransactionCommand, SwapTransaction>()

            .ForMember(dest => dest.StaffUserId, opt => opt.MapFrom(src => src.StaffId))
            .ForMember(dest => dest.CustomerUserId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.OutgoingBatteryId, opt => opt.MapFrom(src => src.OldBatteryId))
            .ForMember(dest => dest.IncomingBatteryId, opt => opt.MapFrom(src => src.NewBatteryId))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Fee))
            .ForMember(dest => dest.SwapStatus, opt => opt.MapFrom(src => src.SwapStatus));

            CreateMap<TranscationDto, SwapTransaction>().ReverseMap();

            CreateMap<SwapTransaction, SwapTranscationFullDto>()
            .ForMember(dest => dest.Station, opt => opt.MapFrom(src => src.Station))
            .ForMember(dest => dest.Reservation, opt => opt.MapFrom(src => src.Reservation))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerUser.FullName))
            .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.StaffUser.FullName))
                .ReverseMap();


            //payment
            CreateMap<Payment, PaymentAndTranDto>()
            .ForMember(dest => dest.Transcation, opt => opt.MapFrom(src => src.SwapTransaction))
                .ReverseMap();

            CreateMap<PaymentUpdateDto, Payment>()
           .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            //station
            CreateMap<Station, StationInSwapDto>().ReverseMap();


            //reservation
            CreateMap<Reservation, ReverInSwapDto>().ReverseMap();

        }
    }
}
