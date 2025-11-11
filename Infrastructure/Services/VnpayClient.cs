using Application.Common.Interfaces.Services;
using Application.Dtos.Payment;
using Domain.Models;
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

        private static string HmacSHA512(string key, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToUpper();
        }
    }
}
