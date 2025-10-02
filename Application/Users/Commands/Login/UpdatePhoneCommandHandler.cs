using Application.Common.Interfaces.Services;
using Domain.Dtos;
using MediatR;

namespace Application.Users.Commands.Login
{
    public class UpdatePhoneCommandHandler : IRequestHandler<UpdatePhoneCommand, LoginResponseDto>
    {
        private readonly IAuthService _authService;

        public UpdatePhoneCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LoginResponseDto> Handle(UpdatePhoneCommand request, CancellationToken cancellationToken)
        {
            return await _authService.UpdatePhoneAsync(request.Email, request.PhoneNumber);
        }
    }
}
