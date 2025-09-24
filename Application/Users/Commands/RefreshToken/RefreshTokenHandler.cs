using Application.Common.Interfaces;
using Domain.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
