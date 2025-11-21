using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Application.Common.Interfaces.Services;
using Application.Common.IRespositories;
using Application.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Jobs;
using Infrastructure.Persistance.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? "")),
                    RoleClaimType = ClaimTypes.Role
                };
            });

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddHttpClient<IOSRMService, OSRMService>(client =>
            {
                client.BaseAddress = new Uri("http://127.0.0.1:5000");
            });

            services.AddQuartz(q =>
            {
                var expireJobKey = new JobKey("ExpireAndHoldBackgroundService");
                q.AddJob<ExpireAndHoldBackgroundService>(opts => opts.WithIdentity(expireJobKey));

                q.AddTrigger(opts => opts
                    .ForJob(expireJobKey)
                    .WithIdentity("ExpireAndHoldBackgroundService-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(1)   
                        .RepeatForever()));

                var cancelUnpaidKey = new JobKey("CancelUnpaidReservationsJob");
                q.AddJob<CancelUnpaidReservationsJob>(opts => opts.WithIdentity(cancelUnpaidKey));

                q.AddTrigger(opts => opts
                    .ForJob(cancelUnpaidKey)
                    .WithIdentity("CancelUnpaidReservationsJob-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(30)   
                        .RepeatForever()));
            });

            services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);

            services.AddScoped<EmailService>();
            services.AddScoped<PaymentRepository>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IBatteryModelRepository, BatteryModelRepository>();
            services.AddTransient<IPaymentRepository, PaymentRepository>();

            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ISubscriptionPackageRepository, SubscriptionPackageRepository>();
            services.AddScoped<IBatteryRepository, BatteryRepository>();
            services.AddScoped<IStationRepository, StationRepository>();
            services.AddScoped<IStationInventoryRepository, StationInventoryRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
            services.AddScoped<ISwapTransactionRepository, SwapTransactionRepository>();
            services.AddScoped<IReservationAllocationRepository, ReservationAllocationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPaymentGatewayClient, VnpayClient>();
            services.AddScoped<IBatteryHealthLogsRepository, BatteryHealthLogsRepository>();
            services.AddScoped<IStationStaffRepository, StationStaffRepository>();
            services.AddScoped<IInterStationTransferRepository, InterStationTransferRepository>();
            return services;
        }
    }
}
