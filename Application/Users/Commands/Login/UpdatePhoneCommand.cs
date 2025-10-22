using Domain.Dtos;
using MediatR;

namespace Application.Users.Commands.Login
{
    public class UpdatePhoneCommand : IRequest<LoginResponseDto>
    {
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
