using MediatR;

namespace Application.Users.Commands.LockUser
{
    public class LockUserCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public LockUserCommand(string userId) => UserId = userId;
    }

    public class UnlockUserCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public UnlockUserCommand(string userId) => UserId = userId;
    }
}
