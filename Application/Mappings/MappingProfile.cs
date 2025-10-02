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

            CreateMap<Driver, RegisterDriverResponse>();

            CreateMap<RegisterSubscriptionRequest, Subscription>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.RemainingSwaps, opt => opt.Ignore());

            CreateMap<Subscription, RegisterSubscriptionResponse>();

            CreateMap<CreatePackageRequest, SubscriptionPackage>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<SubscriptionPackage, SubscriptionPackageDto>();
        }
    }
}
