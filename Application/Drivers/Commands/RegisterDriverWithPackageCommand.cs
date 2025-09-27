using Domain.Dtos;
using MediatR;

namespace Application.Drivers.Commands
{
    public class RegisterDriverWithPackageCommand : IRequest<RegisterDriverWithPackageResponse>
    {
        public string UserId { get; set; }
        public string PreferredPaymentMethod { get; set; }
        public int PackageId { get; set; }
    }
}
