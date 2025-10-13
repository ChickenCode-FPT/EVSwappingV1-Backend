using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpContext? HttpContext => _httpContextAccessor.HttpContext;

        private ClaimsPrincipal? User => HttpContext?.User;

        public string? UserId =>
            User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User?.FindFirstValue("sub"); 

        public string? Email =>
            User?.FindFirstValue(ClaimTypes.Email);

        public IReadOnlyList<string> Roles =>
            User?.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList() ?? new List<string>();

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string roleName) =>
            Roles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
    }
}
