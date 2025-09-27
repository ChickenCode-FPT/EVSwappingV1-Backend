using Domain.Dtos;
using MediatR;

namespace Application.Drivers.Commands
{
    public class RegisterDriverCommand : IRequest<RegisterDriverResponse>
    {
        public string UserId { get; set; }
        public string PreferredPaymentMethod { get; set; }
    }
}
