using Application.Common.Interfaces;
using Application.Dtos.Payment;
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

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetPaymentById(long id)
        {
            var payment = await _paymentService.GetPaymentById(id);
            return payment == null ? NotFound() : Ok(payment);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPayments(string userId)
        {
            return Ok(await _paymentService.GetUserPayments(userId));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyPayments()
        {
            var payments = await _paymentService.GetMyPayments();
            return Ok(payments);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetPaymentAndSwap()
        {
            var payments = await _paymentService.GetPaymentAndSwap();
            return Ok(payments);
        }


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