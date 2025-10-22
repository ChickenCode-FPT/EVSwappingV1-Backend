using Domain.Dtos;
using MediatR;

namespace Application.Users.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<LoginResponseDto>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
