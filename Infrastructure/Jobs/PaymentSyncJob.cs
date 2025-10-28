using Application.Common.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class PaymentSyncJob : IJob
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _config;
        private readonly ILogger<PaymentSyncJob> _logger;
        private readonly IHttpClientFactory _httpFactory;

        private readonly string _vnpApiUrl;
        private readonly string _vnpTmnCode;
        private readonly string _vnpHashSecret;

        public PaymentSyncJob(
            IPaymentRepository paymentRepo,
            IPaymentService paymentService,
            IConfiguration config,
            IHttpClientFactory httpFactory,
            ILogger<PaymentSyncJob> logger)
        {
            _paymentRepo = paymentRepo;
            _paymentService = paymentService;
            _config = config;
            _httpFactory = httpFactory;
            _logger = logger;

            _vnpApiUrl = _config["Vnpay:ApiUrl"] ?? "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";
            _vnpTmnCode = _config["Vnpay:TmnCode"] ?? throw new ArgumentNullException("Vnpay:TmnCode");
            _vnpHashSecret = _config["Vnpay:HashSecret"] ?? throw new ArgumentNullException("Vnpay:HashSecret");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("=== [PaymentSyncJob-VNPAY] Tick at {time} ===", now);

            var pending = (await _paymentRepo.GetAll())
                .Where(p => p.Status == PaymentStatus2.Pending && !string.IsNullOrEmpty(p.TransactionRef))
                .ToList();

            if (!pending.Any())
            {
                _logger.LogInformation("No pending payments to sync.");
                return;
            }

            var client = _httpFactory.CreateClient();

            foreach (var payment in pending)
            {
                try
                {
                    var result = await QueryTransactionAsync(client, payment);
                    if (result == null)
                    {
                        _logger.LogWarning($"[VNPAY] No response for Payment #{payment.PaymentId}");
                        continue;
                    }

                    if (result.vnp_ResponseCode == "00" && result.vnp_TransactionStatus == "00")
                    {
                        payment.Status = PaymentStatus2.Paid;
                        payment.PaidAt = DateTime.UtcNow;
                        await _paymentRepo.Update(payment);
                        await _paymentService.UpdateLinkedEntitiesAfterPayment(payment);
                        _logger.LogInformation($"[VNPAY] Payment #{payment.PaymentId} marked as PAID.");
                    }
                    else if (result.vnp_TransactionStatus == "02" || result.vnp_ResponseCode == "24")
                    {
                        payment.Status = PaymentStatus2.Cancelled;
                        await _paymentRepo.Update(payment);
                        _logger.LogInformation($"[VNPAY] Payment #{payment.PaymentId} CANCELLED (status={result.vnp_TransactionStatus}).");
                    }
                    else
                    {
                        _logger.LogInformation($"[VNPAY] Payment #{payment.PaymentId} still pending (code={result.vnp_ResponseCode}).");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[VNPAY] Error syncing payment #{payment.PaymentId}");
                }
            }

            await _paymentRepo.SaveChanges();
        }

        private async Task<VnpQueryResponse?> QueryTransactionAsync(HttpClient client, Payment payment)
        {
            var requestId = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

            var vnp_Params = new SortedDictionary<string, string>
            {
                ["vnp_RequestId"] = requestId,
                ["vnp_Version"] = "2.1.0",
                ["vnp_Command"] = "querydr",
                ["vnp_TmnCode"] = _vnpTmnCode,
                ["vnp_TxnRef"] = payment.TransactionRef!,
                ["vnp_OrderInfo"] = $"Query payment #{payment.PaymentId}",
                ["vnp_CreateBy"] = "system",
                ["vnp_CreateDate"] = DateTime.Now.ToString("yyyyMMddHHmmss"),
                ["vnp_IpAddr"] = "127.0.0.1"
            };

            var raw = string.Join("&", vnp_Params.Select(kv => $"{kv.Key}={kv.Value}"));
            vnp_Params["vnp_SecureHash"] = ComputeHmac(raw, _vnpHashSecret);

            var response = await client.PostAsync(_vnpApiUrl, new FormUrlEncodedContent(vnp_Params));
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return null;

            return ParsePlainTextResponse(body) ?? JsonSerializer.Deserialize<VnpQueryResponse>(body);
        }

        private static string ComputeHmac(string data, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToUpper();
        }

        private static VnpQueryResponse? ParsePlainTextResponse(string body)
        {
            var dict = body.Split('&')
                .Select(x => x.Split('='))
                .Where(a => a.Length == 2)
                .ToDictionary(a => a[0], a => a[1]);

            return new VnpQueryResponse
            {
                vnp_ResponseCode = dict.GetValueOrDefault("vnp_ResponseCode"),
                vnp_TransactionStatus = dict.GetValueOrDefault("vnp_TransactionStatus"),
                vnp_Message = dict.GetValueOrDefault("vnp_Message")
            };
        }

        private class VnpQueryResponse
        {
            public string? vnp_ResponseCode { get; set; }
            public string? vnp_TransactionStatus { get; set; }
            public string? vnp_Message { get; set; }
            public string? vnp_TxnRef { get; set; }
        }
    }
}
