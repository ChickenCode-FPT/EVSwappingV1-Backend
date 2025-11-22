using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos.Payment;
using Application.Dtos.Swap;
using Application.Interfaces.Repositories;
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
        private readonly IPaymentRepository _paymentRepo;
        private readonly IMapper _mapper;
        private readonly IBatteryRepository _batteryRepo;
        private readonly IReservationAllocationRepository _reservationAllocationRepo;
        private readonly IStationInventoryRepository _inventoryRepo;

        public SwapTransactionService(
            ISwapTransactionRepository swapRepo,
            IReservationRepository reservationRepo,
            ISubscriptionRepository subscriptionRepo,
            IPaymentService paymentService,
            IMapper mapper,
            IPaymentRepository paymentRepo,
            ILogger<SwapTransactionService> logger,
            IBatteryRepository batteryRepo,
            IReservationAllocationRepository reservationAllocationRepo,
            IStationInventoryRepository inventoryRepo)
        {
            _swapRepo = swapRepo;
            _reservationRepo = reservationRepo;
            _subscriptionRepo = subscriptionRepo;
            _paymentService = paymentService;
            _mapper = mapper;
            _paymentRepo = paymentRepo;
            _logger = logger;
            _batteryRepo = batteryRepo;
            _reservationAllocationRepo = reservationAllocationRepo;
            _inventoryRepo = inventoryRepo;
        }

        public async Task<SwapTransactionDto2> ConfirmSwapByStaff(ConfirmSwapByStaffRequest request)
        {
            var swap = await _swapRepo.GetById2(request.SwapTransactionId)
                ?? throw new KeyNotFoundException("Swap transaction not found");

            if (swap.SwapStatus == SwapStatus.Completed)
                throw new InvalidOperationException("Swap already completed.");

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            swap.StaffUserId = request.StaffUserId;
            swap.OutgoingBatteryId = request.OutgoingBatteryId;
            swap.SwapStatus = SwapStatus.InProgress;
            if (!string.IsNullOrEmpty(request.Notes))
                swap.Notes = request.Notes;

            await _swapRepo.Update2(swap);

            var outBat = await _batteryRepo.GetById(request.OutgoingBatteryId);
            if (outBat != null)
            {
                outBat.Status = BatteryStatus.InUse;       
                await _batteryRepo.Update(outBat);
            }

            var outgoingInv = await _inventoryRepo.GetBybatteryId(request.OutgoingBatteryId);
            if (outgoingInv != null)
            {
                outgoingInv.Status = StationInventoryStatus.Empty; 
                outgoingInv.ReservationId = null;
                outgoingInv.CheckedAt = DateTime.UtcNow;
                await _inventoryRepo.Update(outgoingInv);
            }

            await _batteryRepo.SaveChanges();
            await _inventoryRepo.SaveChanges();
            await _swapRepo.SaveChanges();

            scope.Complete();
            return _mapper.Map<SwapTransactionDto2>(swap);
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
            return _mapper.Map<SwapTransactionDto2>(swap);
        }

        public async Task<SwapTransactionDto2> CompleteSwap(CompleteSwapTransactionRequest request)
        {
            var swap = await _swapRepo.GetById2(request.SwapTransactionId)
                ?? throw new KeyNotFoundException("Swap transaction not found");

            if (swap.SwapStatus == SwapStatus.Completed)
                throw new InvalidOperationException("Already completed.");

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            swap.IncomingBatteryId = request.IncomingBatteryId;
            if (!string.IsNullOrEmpty(request.Notes))
                swap.Notes += " | " + request.Notes;
            swap.SwapStatus = SwapStatus.Completed;
            swap.SwapFinishedAt = DateTime.UtcNow;

            await _swapRepo.Update2(swap);

            var incomingBattery = await _batteryRepo.GetById(request.IncomingBatteryId);
            if (incomingBattery != null)
            {
                incomingBattery.Status = BatteryStatus.Empty; 
                await _batteryRepo.Update(incomingBattery);
            }

            var freeSlot = await _inventoryRepo.GetEmptySlot(swap.StationId);

            if (freeSlot != null)
            {
                freeSlot.BatteryId = request.IncomingBatteryId;
                freeSlot.Status = StationInventoryStatus.Full;
                freeSlot.CheckedAt = DateTime.UtcNow;

                await _inventoryRepo.Update(freeSlot);
            }
            else
            {
                var inv = new StationInventory
                {
                    StationId = swap.StationId,
                    BatteryId = request.IncomingBatteryId,
                    SlotNumber = "AUTO",
                    Status = StationInventoryStatus.Full,
                    CheckedAt = DateTime.UtcNow
                };
                await _inventoryRepo.Add(inv);
            }

            if (swap.ReservationId != null)
            {
                var res = await _reservationRepo.GetById(swap.ReservationId.Value);

                if (res != null)
                {
                    res.Status = ReservationStatus.Completed;
                    res.UpdatedAt = DateTime.UtcNow;
                    await _reservationRepo.Update(res);

                    foreach (var alloc in res.ReservationAllocations)
                    {
                        alloc.Status = ReservationAllocationStatus.Consumed;
                        await _reservationAllocationRepo.Update(alloc);
                    }

                    await _reservationAllocationRepo.SaveChanges();
                }
            }

            await _batteryRepo.SaveChanges();
            await _inventoryRepo.SaveChanges();
            await _swapRepo.SaveChanges();
            await _reservationRepo.SaveChanges();

            scope.Complete();
            return _mapper.Map<SwapTransactionDto2>(swap);
        }

        public async Task<bool> DeleteSwap(long id)
        {
            await _swapRepo.Delete2(id);
            return true;
        }

        public async Task<PaymentResponseDto> HandleSwapPayment(long swapTransactionId)
        {
            var swap = await _swapRepo.GetById2(swapTransactionId)
                ?? throw new KeyNotFoundException("Swap transaction not found.");

            if (swap.SwapStatus == SwapStatus.Completed)
                throw new InvalidOperationException("Swap already completed.");

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var activeSub = (await _subscriptionRepo.GetByUser(swap.CustomerUserId))
                .FirstOrDefault(s => s.Status == SubscriptionStatus.Active && s.EndDate >= DateTime.UtcNow);

            if (activeSub != null && activeSub.RemainingSwaps > 0)
            {
                activeSub.RemainingSwaps--;
                await _subscriptionRepo.Update(activeSub);

                swap.SwapStatus = SwapStatus.PaidBySubscription;
                swap.SwapFinishedAt = DateTime.UtcNow;
                swap.Notes += $" | Completed using subscription #{activeSub.SubscriptionId}";

                await _swapRepo.Update(swap);

                scope.Complete();
                return new PaymentResponseDto
                {
                    Success = true,
                    Status = PaymentStatus2.Paid,
                    Description = $"Swap completed using subscription #{activeSub.SubscriptionId}"
                };
            }

            decimal amountToPay = swap.Price;
            Payment? deposit = null;

            if (swap.ReservationId.HasValue)
            {
                var res = await _reservationRepo.GetById(swap.ReservationId.Value);
                deposit = res?.Payments.FirstOrDefault(p =>
                    p.Type == PaymentType.ReservationDeposit &&
                    p.Status == PaymentStatus2.Paid);

                if (deposit != null)
                {
                    amountToPay = Math.Max(0, swap.Price - deposit.Amount);

                    deposit.Status = PaymentStatus2.Forfeit; // trừ cọc
                    await _paymentRepo.Update(deposit);
                }
            }

            if (amountToPay <= 0)
            {
                swap.SwapStatus = SwapStatus.Completed;
                swap.SwapFinishedAt = DateTime.UtcNow;
                swap.Notes += " | Completed using deposit balance.";
                await _swapRepo.Update(swap);

                scope.Complete();
                return new PaymentResponseDto
                {
                    Success = true,
                    Status = PaymentStatus2.Paid,
                    Description = "Swap completed using reservation deposit."
                };
            }

            var paymentRequest = new PaymentCreateDto
            {
                UserId = swap.CustomerUserId,
                SwapTransactionId = swap.SwapTransactionId,
                Type = PaymentType.SwapFee,
                Amount = amountToPay,
                Currency = "VND",
                Description = $"Swap fee for transaction #{swap.SwapTransactionId}",
                Method = "VNPAY"
            };

            var paymentResponse = await _paymentService.CreatePayment(paymentRequest);

            swap.SwapStatus = SwapStatus.PendingPayment;
            swap.Notes += $" | Awaiting payment of {amountToPay:N0} VND";
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
