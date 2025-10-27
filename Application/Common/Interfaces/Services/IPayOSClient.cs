using Application.Dtos;
using Domain.Models;

namespace Application.Common.Interfaces.Services
{
    public interface IPayOSClient
    {
        Task<PaymentResponseDto> CreatePaymentRequestAsync(Payment payment);
        Task<RefundResultDto> CreateRefundAsync(Payment originalPayment, RefundRequestDto request);
        Task<PaymentStatusResponseDto?> GetPaymentStatusAsync(string orderCode);
        bool VerifyWebhookSignature(string rawBody, string signature);
    }
}
