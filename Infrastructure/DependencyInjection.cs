using Application.Common.Interfaces;
using Domain.Models;
using Infrastructure.Persistance.Repositories;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISupportTicketService, SupportTicketService>();
            services.AddScoped<IRatingService, RatingService>();

            services.AddScoped<SupportTicketRepository>();
            services.AddScoped<RatingRepository>();
            return services;
        }
    }
}
