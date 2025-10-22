using MediatR;

namespace Application.Users.Commands.Password
{
    public class ForgotPasswordCommand : IRequest<Unit>
    {
        public string Email { get; set; }
    }

}
