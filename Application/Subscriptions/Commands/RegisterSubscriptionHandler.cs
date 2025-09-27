using Application.Common.Interfaces;
using Domain.Dtos;
using MediatR;

namespace Application.Subscriptions.Commands
{
    public class RegisterSubscriptionHandler : IRequestHandler<RegisterSubscriptionCommand, RegisterSubscriptionResponse>
    {
        private readonly ISubscriptionService _subscriptionService;

        public RegisterSubscriptionHandler(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public async Task<RegisterSubscriptionResponse> Handle(RegisterSubscriptionCommand request, CancellationToken cancellationToken)
        {
            return await _subscriptionService.RegisterSubscriptionAsync(request.UserId, request.PackageId);
        }
    }
}
