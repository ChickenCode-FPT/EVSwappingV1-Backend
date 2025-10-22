using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Users.Commands.Login
{
    public class GoogleLoginCommand : IRequest<GoogleLoginResponseDto>
    {
        public HttpContext HttpContext { get; set; }

        public GoogleLoginCommand(HttpContext httpContext)
        {
            HttpContext = httpContext;
        }
    }
}
