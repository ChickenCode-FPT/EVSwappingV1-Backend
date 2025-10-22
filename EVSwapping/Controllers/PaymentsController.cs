using Application.Common.Interfaces;
using Application.Dtos;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: api/payments
        [HttpPost]
        public async Task<IActionResult> AddPayment([FromBody] Payment payment)
        {
            if (payment == null)
            {
                return BadRequest("Payment cannot be null");
            }

            try
            {
                await _paymentService.AddPayment(payment);
                return CreatedAtAction(nameof(GetPaymentById), new { id = payment.PaymentId }, payment);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        // GET: api/payments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentById(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        // GET: api/payments
        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _paymentService.GetAllPayments();
            return Ok(payments);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetAllAndSwap()
        {
            var payments = await _paymentService.GetFilterWithSwapt();
            return Ok(payments);
        }

        // PUT: api/payments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] PaymentUpdateDto payment)
        {
            if (id != payment.PaymentId)
            {
                return BadRequest("Payment ID mismatch");
            }

            try
            {
                await _paymentService.UpdatePayment(id, payment);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        // DELETE: api/payments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            try
            {
                await _paymentService.DeletePayment(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        // POST: api/payments/return
        [HttpPost("return")]
        public async Task<IActionResult> HandleReturnTransaction([FromBody] ReturnTransactionDto dto)
        {
            if (dto == null || dto.SwapTransactionId <= 0 || dto.BatteryId <= 0)
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                await _paymentService.HandleReturnTransactionAsync(dto.SwapTransactionId, dto.BatteryId, dto.ReturnCondition);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        public class ReturnTransactionDto
        {
            public int SwapTransactionId { get; set; }
            public int BatteryId { get; set; }
            public string ReturnCondition { get; set; }
        }
    }
}