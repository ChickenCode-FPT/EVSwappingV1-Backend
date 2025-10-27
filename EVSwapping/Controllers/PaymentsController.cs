using Application.Common.Interfaces;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _paymentService.CreatePayment(dto);
            return Ok(result);
        }

        [HttpPost("penalty")]
        public async Task<IActionResult> CreatePenalty([FromBody] PenaltyPaymentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _paymentService.CreatePenalty(dto);
            return Ok(result);
        }

        [HttpPost("refund")]
        public async Task<IActionResult> CreateRefund([FromBody] RefundRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _paymentService.CreateRefund(dto);
            return Ok(result);
        }

        [HttpGet("vnpay-callback")]
        public async Task<IActionResult> VnpayCallback()
        {
            var query = Request.Query;
            var dto = new PaymentWebhookDto
            {
                OrderCode = query["vnp_TxnRef"],
                Status = query["vnp_ResponseCode"] == "00" ? "PAID" : "FAILED",
                Amount = decimal.Parse(query["vnp_Amount"]) / 100,
                Signature = query["vnp_SecureHash"],
                RawData = query.ToString()
            };

            var result = await _paymentService.HandleWebhook(dto);
            if (result == null)
                return NotFound(new { message = "Payment not found for VNPAY transaction." });

            return Redirect($"https://app.ev-swap.vn/payment-success?order={dto.OrderCode}&status={dto.Status}");
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncPendingPayments()
        {
            await _paymentService.SyncPendingPaymentsAsync();
            return Ok(new { message = "Pending payments synchronized successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments() => Ok(await _paymentService.GetAllPayments());

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetPaymentById(long id)
        {
            var payment = await _paymentService.GetPaymentById(id);
            return payment == null ? NotFound() : Ok(payment);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPayments(string userId)
            => Ok(await _paymentService.GetUserPayments(userId));

        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] PaymentStatusUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _paymentService.UpdatePaymentStatus(dto);

            if (result == null)
                return NotFound(new { message = "Payment not found." });

            return Ok(result);
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok(new
        {
            service = "PaymentService",
            version = "v3.0",
            gateway = "VNPAY",
            timestamp = DateTime.UtcNow,
            message = "Payment system operational (VNPAY integrated)."
        });
    }
}