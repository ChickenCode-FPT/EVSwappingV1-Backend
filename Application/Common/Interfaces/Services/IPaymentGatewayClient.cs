using Application.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
