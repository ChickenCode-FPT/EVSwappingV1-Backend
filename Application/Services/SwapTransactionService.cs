using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace Application.Services
{
    public class SwapTransactionService : ISwapTransactionService
    {
        private readonly ISwapTransactionRepository _swapRepo;
        private readonly IReservationRepository _reservationRepo;
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<SwapTransactionService> _logger;
        private readonly IMapper _mapper;

        public SwapTransactionService(
            ISwapTransactionRepository swapRepo,
            IReservationRepository reservationRepo,
            ISubscriptionRepository subscriptionRepo,
            IPaymentService paymentService,
            IMapper mapper,
            ILogger<SwapTransactionService> logger)
        {
            _swapRepo = swapRepo;
            _reservationRepo = reservationRepo;
            _subscriptionRepo = subscriptionRepo;
            _paymentService = paymentService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SwapTransactionDto2>> GetAll2()
        {
            var swaps = await _swapRepo.GetAll2();
            return _mapper.Map<IEnumerable<SwapTransactionDto2>>(swaps);
        }

        public async Task<SwapTransactionDto2?> GetById2(long id)
        {
            var swap = await _swapRepo.GetById2(id);
            return _mapper.Map<SwapTransactionDto2>(swap);
        }

        public async Task<IEnumerable<SwapTransactionDto2>> GetByUser(string userId)
        {
            var swaps = await _swapRepo.GetByUser(userId);
            return _mapper.Map<IEnumerable<SwapTransactionDto2>>(swaps);
        }

        public async Task<SwapTransactionDto2> CreateSwap(CreateSwapTransactionRequest request)
        {
            var swap = new SwapTransaction
            {
                ReservationId = request.ReservationId,
                StationId = request.StationId,
                CustomerUserId = request.CustomerUserId,
                OutgoingBatteryId = request.OutgoingBatteryId,
                IncomingBatteryId = request.IncomingBatteryId,
                SwapStartedAt = DateTime.UtcNow,
                SwapStatus = SwapStatus.Pending,
                Notes = request.Notes ?? string.Empty,
                Price = request.Price,
                CreatedAt = DateTime.UtcNow
            };

            await _swapRepo.Add2(swap);
            _logger.LogInformation("[Swap] Created new swap #{id} for user {user}", swap.SwapTransactionId, swap.CustomerUserId);
            return _mapper.Map<SwapTransactionDto2>(swap);
        }

        public async Task<SwapTransactionDto2> CompleteSwap(CompleteSwapTransactionRequest request)
        {
            var swap = await _swapRepo.GetById2(request.SwapTransactionId)
                ?? throw new KeyNotFoundException("Swap transaction not found");

            swap.IncomingBatteryId ??= request.IncomingBatteryId;
            swap.SwapFinishedAt = DateTime.UtcNow;
            swap.Price = request.FinalPrice ?? swap.Price;
            swap.SwapStatus = SwapStatus.Completed;
            swap.Notes = request.Notes ?? swap.Notes;

            await _swapRepo.Update2(swap);
            _logger.LogInformation("[Swap] Completed swap #{id}", swap.SwapTransactionId);

            return _mapper.Map<SwapTransactionDto2>(swap);
        }

        public async Task<bool> DeleteSwap(long id)
        {
            await _swapRepo.Delete2(id);
            _logger.LogInformation("[Swap] Deleted swap #{id}", id);
            return true;
        }

        public async Task<PaymentResponseDto> HandleSwapPayment(long swapTransactionId)
        {
            var swap = await _swapRepo.GetById(swapTransactionId)
                ?? throw new KeyNotFoundException("Swap transaction not found.");

            if (swap.SwapStatus == SwapStatus.Completed)
                throw new InvalidOperationException("Swap already completed.");

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var activeSub = (await _subscriptionRepo.GetByUser(swap.CustomerUserId))
                .FirstOrDefault(s => s.Status == SubscriptionStatus.Active && s.EndDate >= DateTime.UtcNow);

            if (activeSub != null)
            {
                if (activeSub.RemainingSwaps > 0)
                {
                    activeSub.RemainingSwaps--;
                    await _subscriptionRepo.Update(activeSub);

                    swap.SwapStatus = SwapStatus.Completed;
                    swap.SwapFinishedAt = DateTime.UtcNow;
                    swap.Notes = $"Completed using subscription #{activeSub.SubscriptionId}";
                    await _swapRepo.Update(swap);

                    _logger.LogInformation("[Swap] Completed by subscription for user {user}", swap.CustomerUserId);

                    scope.Complete();

                    return new PaymentResponseDto
                    {
                        Success = true,
                        Description = "Swap completed using active subscription.",
                        Status = PaymentStatus2.Paid
                    };
                }

                _logger.LogInformation("[Swap] Subscription expired or no remaining swaps, creating payment...");
            }

            var paymentRequest = new PaymentCreateDto
            {
                UserId = swap.CustomerUserId,
                SwapTransactionId = swap.SwapTransactionId,
                Type = PaymentType.SwapFee,
                Amount = swap.Price,
                Currency = "VND",
                Description = $"Swap fee for transaction #{swap.SwapTransactionId}",
                Method = "VNPAY"
            };

            var paymentResponse = await _paymentService.CreatePayment(paymentRequest);

            swap.SwapStatus = SwapStatus.PendingPayment;
            await _swapRepo.Update(swap);

            scope.Complete();
            return paymentResponse;
        }

        public async Task<SwapTransaction?> GetById(long id)
        {
            await Task.Delay(1);

            return null;
        }

        public async Task<List<SwapTransaction>> GetAll()
        {
            await Task.Delay(1);

            return null;
        }

        public async Task Add(SwapTransaction transaction)
        {
            await Task.Delay(1);
        }

        public async Task Update(SwapTransaction transaction)
        {
            await Task.Delay(1);
        }

        public async Task<List<SwapTransaction>> GetAllWithStationAndReversation()
        {
            await Task.Delay(1);

            return null;
        }

        public async Task<SwapTransaction?> GetAllWithStationAndReversationID(int id)
        {
            await Task.Delay(1);

            return null;
        }
    }
}
