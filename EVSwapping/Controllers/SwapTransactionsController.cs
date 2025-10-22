using Application.SwapTransactions.Commands;
using Application.SwapTransactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/swapTransactions")]
    public class SwapTransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SwapTransactionsController(IMediator mediator) => _mediator = mediator;

        // POST: api/swapTransactions
        [HttpPost]
        public async Task<IActionResult> CreateSwapTransaction([FromBody] CreateSwapTransactionCommand command)
        {
            if (command == null)
                return BadRequest("Invalid data.");

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetSwapTransactionById), new { id = result }, result);
        }

        // PUT: api/swapTransactions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSwapTransaction(long id, [FromBody] UpdateSwapTransactionCommand command)
        {
            if (command == null || id != command.SwapTransactionId)
                return BadRequest("Invalid data.");

            var result = await _mediator.Send(command);

            return NoContent();
        }

        
        // GET: api/swapTransactions/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSwapTransactionById(long id)
        {
            var query = new GetSwapTransactionByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound("Swap transaction not found.");

            return Ok(result);
        }
        
        // GET: api/swapTransactions
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
    }
}
