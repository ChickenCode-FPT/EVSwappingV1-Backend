using Application.Common.Interfaces.Services;
using Application.Dtos.Swap;
using Application.SwapTransactions.Commands;
using Application.SwapTransactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/swapTransactions")]
    public class SwapTransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISwapTransactionService _swapService;
        private readonly ILogger<SwapTransactionsController> _logger;

        public SwapTransactionsController(
            ISwapTransactionService swapService,
            ILogger<SwapTransactionsController> logger,
            IMediator mediator)
        {
            _swapService = swapService;
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSwapTransaction([FromBody] CreateSwapTransactionCommand command)
        {
            if (command == null)
                return BadRequest("Invalid data.");

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetSwapTransactionById), new { id = result }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSwapTransaction(long id, [FromBody] UpdateSwapTransactionCommand command)
        {
            if (command == null || id != command.SwapTransactionId)
                return BadRequest("Invalid data.");

            var result = await _mediator.Send(command);

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSwapTransactionById(long id)
        {
            var query = new GetSwapTransactionByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound("Swap transaction not found.");

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSwapTransactions()
        {
            var query = new GetAllSwapTransactionsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetFullSwapTransactions()
        {
            var query = new GetFullSwapTransactionQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("full/{id}")]
        public async Task<IActionResult> GetFullSwapTransaction(int id)
        {
            var query = new GetFullSwapTransactionIDQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }




        [HttpGet("v2")]
        public async Task<IActionResult> GetAll()
        {
            var swaps = await _swapService.GetAll2();
            return Ok(swaps);
        }

        [HttpGet("v2/{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var swap = await _swapService.GetById2(id);
            if (swap == null)
                return NotFound(new { message = $"SwapTransaction #{id} not found" });

            return Ok(swap);
        }

        [HttpGet("v2/user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var swaps = await _swapService.GetByUser(userId);
            return Ok(swaps);
        }

        [HttpPost("v2")]
        public async Task<IActionResult> Create([FromBody] CreateSwapTransactionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _swapService.CreateSwap(request);

            return Ok(result);
        }

        [HttpPost("v2/complete")]
        public async Task<IActionResult> Complete([FromBody] CompleteSwapTransactionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _swapService.CompleteSwap(request);

            return Ok(result);
        }

        [HttpPost("v2/{swapId:long}/handle-payment")]
        public async Task<IActionResult> HandleSwapPayment(long swapId)
        {
            try
            {
                var result = await _swapService.HandleSwapPayment(swapId);

                if (result.Success && string.IsNullOrEmpty(result.CheckoutUrl))
                {
                    return Ok(new
                    {
                        message = "Swap completed via subscription.",
                        paymentStatus = result.Status,
                        swapId
                    });
                }

                return Ok(new
                {
                    message = "Swap fee requires payment.",
                    checkoutUrl = result.CheckoutUrl,
                    paymentId = result.PaymentId,
                    paymentStatus = result.Status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("v2/{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _swapService.DeleteSwap(id);
            if (!success)
                return NotFound(new { message = $"Swap #{id} not found or could not be deleted." });

            return Ok(new { message = $"Swap #{id} deleted successfully." });
        }

        [HttpPost("v2/confirm")]
        public async Task<IActionResult> ConfirmSwap([FromBody] ConfirmSwapByStaffRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _swapService.ConfirmSwapByStaff(request);
            return Ok(result);
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                service = "SwapTransactionService",
                version = "v2.1",
                timestamp = DateTime.UtcNow,
                status = "Operational"
            });
        }
    }
}
