using Domain.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<LoginResponseDto>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
