using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.Password
{
    public class ForgotPasswordCommand : IRequest<Unit>
    {
        public string Email { get; set; }
    }

}
