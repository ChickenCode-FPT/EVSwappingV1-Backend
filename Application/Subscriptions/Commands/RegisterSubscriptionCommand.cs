using Domain.Dtos;
using MediatR;

namespace Application.Subscriptions.Commands
{
    public class RegisterSubscriptionCommand : IRequest<RegisterSubscriptionResponse>
    {
        public string UserId { get; set; }
        public int PackageId { get; set; }
    }
}
