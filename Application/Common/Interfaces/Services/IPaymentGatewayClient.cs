using Application.Dtos.Payment;
using Domain.Models;

namespace Application.Common.Interfaces.Services
{
    public interface IPaymentGatewayClient
    {
        Task<PaymentResponseDto> CreatePaymentRequestAsync(Payment payment);
    }
}
