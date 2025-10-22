using Domain.Dtos;
using MediatR;

namespace Application.Users.Commands.Login
{
    public class LoginUserCommand : IRequest<LoginResponseDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}