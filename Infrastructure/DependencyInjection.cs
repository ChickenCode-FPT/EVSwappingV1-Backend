using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Domain.Models;
using Infrastructure.Persistance.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<EVSwappingV2Context>(options =>
               options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            // Identity
            services.AddIdentity<User, IdentityRole>()
                     .AddEntityFrameworkStores<EVSwappingV2Context>()
                    .AddDefaultTokenProviders();

            // JWT Auth
            var jwtKey = config["Jwt:Key"];
            var jwtIssuer = config["Jwt:Issuer"];

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    RoleClaimType = ClaimTypes.Role
                };
            });

            services.AddScoped<EmailService>();
            services.AddScoped<PaymentRepository>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ISubscriptionPackageRepository, SubscriptionPackageRepository>();
            services.AddScoped<IBatteryRepository, BatteryRepository>();
            services.AddScoped<IStationRepository, StationRepository>();
            services.AddScoped<IStationInventoryRepository, StationInventoryRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();

            return services;
        }
    }
}
