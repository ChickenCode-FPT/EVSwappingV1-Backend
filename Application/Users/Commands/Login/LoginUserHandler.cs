using Application.Common.Interfaces.Services;
using Domain.Dtos;
using MediatR;

namespace Application.Users.Commands.Login
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
    {
        private readonly IAuthService _authService;

        public LoginUserHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var token = await _authService.LoginAsync(request.Email, request.Password);
            return token;
        }
    }

}
