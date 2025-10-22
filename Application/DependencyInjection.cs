using Application.Common.Interfaces;
using Application.Dtos;
using Application.Mappings;
using Application.Services;
using Domain.Models;
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

            services.AddAutoMapper(typeof(AppProfile).Assembly);

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IBatteryModelService, BatteryModelService>();

            return services;
        }
    }
}
