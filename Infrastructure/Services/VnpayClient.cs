using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class VnpayClient : IPaymentGatewayClient
    {
        private readonly IConfiguration _config;
        private readonly ILogger<VnpayClient> _logger;

        private readonly string _vnpTmnCode;
        private readonly string _vnpHashSecret;
        private readonly string _vnpBaseUrl;
        private readonly string _vnpReturnUrl;
        private readonly string _vnpApiUrl;

        public VnpayClient(IConfiguration config, ILogger<VnpayClient> logger)
        {
            _config = config;
            _logger = logger;

            _vnpTmnCode = _config["Vnpay:TmnCode"] ?? throw new ArgumentNullException("Vnpay:TmnCode");
            _vnpHashSecret = _config["Vnpay:HashSecret"] ?? throw new ArgumentNullException("Vnpay:HashSecret");
            _vnpBaseUrl = _config["Vnpay:BaseUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            _vnpReturnUrl = _config["Vnpay:ReturnUrl"] ?? "http://localhost:4200/payment/result";
            _vnpApiUrl = _config["Vnpay:ApiUrl"] ?? "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";
        }

        public async Task<PaymentResponseDto> CreatePaymentRequestAsync(Payment payment)
        {
            var createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var expireDate = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");
            var orderRef = payment.TransactionRef;

            var vnp_Params = new SortedList<string, string>
            {
                ["vnp_Version"] = "2.1.0",
                ["vnp_Command"] = "pay",
                ["vnp_TmnCode"] = _vnpTmnCode,
                ["vnp_Amount"] = ((int)(payment.Amount * 100)).ToString(),
                ["vnp_CreateDate"] = createDate,
                ["vnp_ExpireDate"] = expireDate,
                ["vnp_CurrCode"] = "VND",
                ["vnp_IpAddr"] = "127.0.0.1",
                ["vnp_Locale"] = "vn",
                ["vnp_OrderInfo"] = payment.Description ?? $"Thanh toan don hang {payment.PaymentId}",
                ["vnp_OrderType"] = "other",
                ["vnp_ReturnUrl"] = _vnpReturnUrl,
                ["vnp_TxnRef"] = orderRef
            };

            var rawData = string.Join("&", vnp_Params.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));

            var secureHash = HmacSHA512(_vnpHashSecret, rawData);

            var queryUrl = string.Join("&", vnp_Params.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
            var paymentUrl = $"{_vnpBaseUrl}?{queryUrl}&vnp_SecureHash={secureHash}";

            _logger.LogInformation("[VNPAY] rawData={raw}", rawData);
            _logger.LogInformation("[VNPAY] SecureHash={hash}", secureHash);
            _logger.LogInformation("[VNPAY] Checkout URL: {url}", paymentUrl);

            return new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                UserId = payment.UserId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Method = "VNPAY",
                Status = "Pending",
                Description = payment.Description,
                CreatedAt = payment.CreatedAt,
                CheckoutUrl = paymentUrl,
                GatewayOrderCode = orderRef,
                Success = true
            };
        }

        public bool ValidateCallbackSignature(IQueryCollection query)
        {
            var sorted = query
                .Where(k => k.Key.StartsWith("vnp_") &&
                            k.Key != "vnp_SecureHash" &&
                            k.Key != "vnp_SecureHashType")
                .OrderBy(k => k.Key)
                .ToDictionary(k => k.Key, k => k.Value.ToString());

            var rawData = string.Join("&", sorted.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var expectedHash = HmacSHA512(_vnpHashSecret, rawData);
            var receivedHash = query["vnp_SecureHash"].ToString();

            var valid = expectedHash.Equals(receivedHash, StringComparison.OrdinalIgnoreCase);

            if (!valid)
            {
                _logger.LogWarning("[VNPAY] ❌ Sai chữ ký callback!");
                _logger.LogWarning("Expected: {expected}", expectedHash);
                _logger.LogWarning("Received: {received}", receivedHash);
                _logger.LogWarning("RawData: {raw}", rawData);
            }
            else
            {
                _logger.LogInformation("[VNPAY] ✅ Chữ ký hợp lệ cho TxnRef={ref}", query["vnp_TxnRef"]);
            }

            return valid;
        }

        public PaymentWebhookDto ParseCallback(IQueryCollection query)
        {
            var status = query["vnp_ResponseCode"] == "00" ? "PAID" : "FAILED";
            return new PaymentWebhookDto
            {
                OrderCode = query["vnp_TxnRef"],
                Status = status,
                Amount = decimal.Parse(query["vnp_Amount"]) / 100,
                RawData = query.ToString(),
                Signature = query["vnp_SecureHash"]
            };
        }

        public async Task<PaymentStatusResponseDto?> GetPaymentStatusAsync(string orderCode)
        {
            await Task.Delay(50);
            return new PaymentStatusResponseDto
            {
                Code = "00",
                Status = "PAID",
                Amount = 0
            };
        }

        public async Task<RefundResultDto> CreateRefundAsync(Payment originalPayment, RefundRequestDto request)
        {
            var refundRef = $"RF{DateTime.Now:yyyyMMddHHmmss}";
            var vnp_Params = new SortedList<string, string>
            {
                ["vnp_RequestId"] = refundRef,
                ["vnp_Version"] = "2.1.0",
                ["vnp_Command"] = "refund",
                ["vnp_TmnCode"] = _vnpTmnCode,
                ["vnp_TransactionType"] = "02",
                ["vnp_TxnRef"] = originalPayment.TransactionRef ?? originalPayment.PaymentId.ToString(),
                ["vnp_Amount"] = ((int)(request.RefundAmount * 100)).ToString(),
                ["vnp_OrderInfo"] = request.Reason,
                ["vnp_CreateBy"] = request.StaffUserId ?? "system",
                ["vnp_CreateDate"] = DateTime.Now.ToString("yyyyMMddHHmmss")
            };

            var raw = string.Join("&", vnp_Params.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var secureHash = HmacSHA512(_vnpHashSecret, raw);
            vnp_Params["vnp_SecureHash"] = secureHash;

            var client = new HttpClient();
            var content = new FormUrlEncodedContent(vnp_Params);
            var response = await client.PostAsync(_vnpApiUrl, content);
            var body = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("[VNPAY] Refund API response: {body}", body);

            var ok = response.IsSuccessStatusCode && body.Contains("00");
            return new RefundResultDto
            {
                Success = ok,
                Message = ok ? "Refund success (sandbox)" : "Refund failed",
                GatewayReference = refundRef
            };
        }

        private static string HmacSHA512(string key, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToUpper();
        }
    }
}
