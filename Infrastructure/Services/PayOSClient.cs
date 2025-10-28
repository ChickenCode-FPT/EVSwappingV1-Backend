using Application.Common.Interfaces.Services;
using Application.Dtos.Payment;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class PayOSClient : IPayOSClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PayOSClient> _logger;
        private readonly IConfiguration _config;

        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly string _checksumKey;
        private readonly string _partnerCode;

        public PayOSClient(HttpClient httpClient, ILogger<PayOSClient> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = config;

            _baseUrl = _config["PayOS:BaseUrl"] ?? "https://api.payos.vn";
            _apiKey = _config["PayOS:ApiKey"] ?? "DEMO-KEY";
            _checksumKey = _config["PayOS:ChecksumKey"] ?? "DEMO-CHECKSUM";
            _partnerCode = _config["PayOS:PartnerCode"] ?? "DEMO-PARTNER";

            _httpClient.DefaultRequestHeaders.Add("x-client-id", _partnerCode);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        }

        public async Task<PaymentResponseDto> CreatePaymentRequestAsync(Payment payment)
        {
            var data = new
            {
                orderCode = payment.PaymentId,
                amount = (int)payment.Amount,
                description = payment.Description ?? $"Payment #{payment.PaymentId}",
                cancelUrl = "https://yourapp.com/payment/cancel",
                returnUrl = "https://yourapp.com/payment/success",
                signature = GenerateSignature(payment)
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/v2/payment-requests", data);
            var json = await response.Content.ReadFromJsonAsync<PayOSPaymentResponse>();

            if (!response.IsSuccessStatusCode || json?.code != "00")
                throw new Exception($"PayOS create payment failed: {json?.desc}");

            return new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                UserId = payment.UserId,
                Status = payment.Status,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Method = payment.Method,
                Description = payment.Description,
                CheckoutUrl = json.data.checkoutUrl,
                //PayOSOrderCode = json.data.paymentLinkId,
                CreatedAt = payment.CreatedAt
            };
        }

        public async Task<RefundResultDto> CreateRefundAsync(Payment originalPayment, RefundRequestDto request)
        {
            try
            {
                var payout = new
                {
                    referenceId = $"refund_{originalPayment.PaymentId}_{DateTime.UtcNow.Ticks}",
                    amount = (int)request.RefundAmount,
                    description = request.Reason,
                    toBin = "970415", // Bank code demo
                    toAccountNumber = "123456789", // Bank acc demo
                    category = new[] { "refund" }
                };

                var jsonBody = System.Text.Json.JsonSerializer.Serialize(payout);
                var signature = ComputeHmac(jsonBody, _checksumKey);

                var req = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v1/payouts");
                req.Headers.Add("x-idempotency-key", payout.referenceId);
                req.Headers.Add("x-signature", signature);
                req.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(req);
                var respBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"PayOS refund success: {respBody}");
                    //return new RefundResultDto { Success = true, Message = "Refund success", PayOSReference = payout.referenceId };
                    return new RefundResultDto { Success = true, Message = "Refund success" };
                }

                _logger.LogWarning($"PayOS refund failed: {respBody}");
                return new RefundResultDto { Success = false, Message = respBody };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling PayOS refund API");
                return new RefundResultDto { Success = false, Message = ex.Message };
            }
        }

        public async Task<PaymentStatusResponseDto?> GetPaymentStatusAsync(string orderCode)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/v2/payment-requests/{orderCode}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadFromJsonAsync<PayOSPaymentStatusResponse>();
            return new PaymentStatusResponseDto
            {
                Code = json?.code,
                Status = json?.data.status,
                Amount = json?.data.amount ?? 0
            };
        }

        public bool VerifyWebhookSignature(string rawBody, string signature)
        {
            var computed = ComputeHmac(rawBody, _checksumKey);
            return computed.Equals(signature, StringComparison.OrdinalIgnoreCase);
        }

        private string GenerateSignature(Payment payment)
        {
            var raw = $"amount={payment.Amount}&cancelUrl=https://yourapp.com/payment/cancel&description={payment.Description}&orderCode={payment.PaymentId}&returnUrl=https://yourapp.com/payment/success";
            return ComputeHmac(raw, _checksumKey);
        }

        private static string ComputeHmac(string data, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private class PayOSPaymentResponse
        {
            public string code { get; set; }
            public string desc { get; set; }
            public dynamic data { get; set; }
        }

        private class PayOSPaymentStatusResponse
        {
            public string code { get; set; }
            public string desc { get; set; }
            public PaymentStatusData data { get; set; }
        }

        private class PaymentStatusData
        {
            public string status { get; set; }
            public int amount { get; set; }
        }
    }
}
