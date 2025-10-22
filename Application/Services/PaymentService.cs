using Application.Common.Interfaces;
using Application.Common.IRespositories;
using Application.Dtos;
using AutoMapper;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBatteryService _batteryRepository;
        private readonly ISwapTransactionService _swapTransactionRepository;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepository, IBatteryService batteryRepository, ISwapTransactionService swapTransactionRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _batteryRepository = batteryRepository;
            _swapTransactionRepository = swapTransactionRepository;
            _mapper = mapper;
        }

        // CRUD Operations
        public async Task AddPayment(Payment payment)
        {
            await _paymentRepository.Add(payment);
        }

        public async Task<PaymentAndTranDto?> GetPaymentById(int id)
        {
            var raw = await _paymentRepository.GetById(id);
            return _mapper.Map<PaymentAndTranDto>(raw);
        }

        public async Task<List<Payment>> GetAllPayments()
        {
            return await _paymentRepository.GetAll();
        }

        public async Task UpdatePayment(int id, PaymentUpdateDto dto)
        {
            var existing = await _paymentRepository.GetById(id);
            if (existing == null)
                throw new Exception("Payment not found");

            _mapper.Map(dto, existing);

            await _paymentRepository.Update(existing);
        }

        public async Task DeletePayment(int id)
        {
            await _paymentRepository.Delete(id);
        }

        public async Task HandleReturnTransactionAsync(int swapTransactionId, int batteryId, string returnCondition)
        {
            // Kiểm tra payment
            var payment = await _paymentRepository.GetById(swapTransactionId);
            if (payment == null || payment.Status != "Completed")
            {
                throw new Exception("Payment not completed, cannot return battery");
            }

            // Tạo transaction mới cho việc hoàn trả
            var transaction = new SwapTransaction
            {
                OutgoingBatteryId = null,
                IncomingBatteryId = batteryId,
                StationId = payment.SwapTransaction.StationId,
                SwapStatus = "Returned",
                CreatedAt = DateTime.UtcNow
            };

            // Lưu transaction mới
            await _swapTransactionRepository.Add(transaction);

            // Cập nhật trạng thái của battery
            var battery = await _batteryRepository.GetById(batteryId);
            if (battery != null)
            {
                battery.Status = returnCondition;
                await _batteryRepository.Update(battery);
            }
            else
            {
                throw new Exception("Battery not found.");
            }
        }

        public async Task<IEnumerable<PaymentAndTranDto>> GetFilterWithSwapt()
        {
            var raw = await _paymentRepository.GetFilterWithSwapt();
            return _mapper.Map<List<PaymentAndTranDto>>(raw);
        }
    }
}
