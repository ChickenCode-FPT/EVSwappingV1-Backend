using Application.Dtos.Payment;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Services
{
    public interface IPaymentGatewayClient
    {
        Task<PaymentResponseDto> CreatePaymentRequestAsync(Payment payment);
        Task<RefundResultDto> CreateRefundAsync(Payment originalPayment, RefundRequestDto request);
        Task<PaymentStatusResponseDto?> GetPaymentStatusAsync(string orderCode);
        bool ValidateCallbackSignature(IQueryCollection query);
        PaymentWebhookDto ParseCallback(IQueryCollection query);
    }
}
