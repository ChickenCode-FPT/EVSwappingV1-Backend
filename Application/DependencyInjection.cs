using Application.Common.Interfaces.Services;
using Application.Mappings;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddAutoMapper(typeof(AppProfile));

            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<ISubscriptionPackageService, SubscriptionPackageService>();
            services.AddScoped<IBatteryService, BatteryService>();
            services.AddScoped<IStationService, StationService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<ISupportTicketService, SupportTicketService>();

            return services;
        }
    }
}
