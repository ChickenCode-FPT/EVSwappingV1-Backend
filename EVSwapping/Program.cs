using System.Text;
using Application;
using Infrastructure;
using Infrastructure.Seeder;
using Infrastructure.Settings;
using Microsoft.OpenApi.Models;
using Serilog; 
using Infrastructure.Monitoring;

var builder = WebApplication.CreateBuilder(args);

// Serilog
//builder.Host.UseSerilog((context, config) =>
//{
//    config.MinimumLevel.Debug()
//          .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
//          .WriteTo.File("logs/quartz-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
//          .Enrich.FromLogContext();
//});

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EVSwapping API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your Bearer '<token>' Ex: 'Bearer eyJhbGciOiJI...'"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Google OAuth
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie("Cookies")
.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.Scope.Add("email");
    options.Scope.Add("profile");
    options.SaveTokens = true;
});

// SMTP & Infrastructure
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Build & Seed
var app = builder.Build();
await AppSeeder.SeedAllAsync(app.Services);

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//app.UseQuartzLogViewer();

app.Run();
