using Application.Common.Interfaces.Services;
using Domain.Dtos;
using MediatR;

namespace Application.Users.Commands.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDto>
    {
        private readonly IAuthService _authService;

        public RefreshTokenHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LoginResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            return result;
        }
    }
}
