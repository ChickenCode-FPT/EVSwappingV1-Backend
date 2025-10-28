namespace Application.Dtos.Payment
{
    public class PaymentDto
    {
        public long PaymentId { get; set; }
        public long? SwapTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public string? Method { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime? PaidAt { get; set; }
        public string? CheckoutUrl { get; set; }
    }

    public class PaymentAndTranDto
    {
        public long PaymentId { get; set; }
        public long? SwapTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public string? Method { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime? PaidAt { get; set; }
        public string? CheckoutUrl { get; set; }
        public TranscationDto Transcation { get; set; } = new();
    }

    public class TranscationDto
    {
        public long SwapTransactionId { get; set; }
        public int? ReservationId { get; set; }
        public int StationId { get; set; }
        public string CustomerUserId { get; set; } = string.Empty;
        public string? StaffUserId { get; set; }
        public int? OutgoingBatteryId { get; set; }
        public int? IncomingBatteryId { get; set; }
        public DateTime SwapStartedAt { get; set; }
        public DateTime? SwapFinishedAt { get; set; }
        public string SwapStatus { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentCreateDto
    {
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public string Method { get; set; } = "VNPAY";
        public string Type { get; set; } = Domain.Enums.PaymentType.SwapFee;
        public string Description { get; set; } = string.Empty;
        public long? SwapTransactionId { get; set; }
        public int? ReservationId { get; set; }
        public int? SubscriptionId { get; set; }
        public string? TransactionRef { get; set; }
    }

    public class PaymentResponseDto
    {
        public long PaymentId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public string Method { get; set; } = "VNPAY";
        public string? Description { get; set; }
        public bool Success { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public long? SwapTransactionId { get; set; }
        public int? ReservationId { get; set; }
        public int? SubscriptionId { get; set; }
        public string? GatewayOrderCode { get; set; }
        public string? CheckoutUrl { get; set; }
    }

    public class PaymentWebhookDto
    {
        public string OrderCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Signature { get; set; } = string.Empty;
        public string RawData { get; set; } = string.Empty;
    }

    public class RefundRequestDto
    {
        public long OriginalPaymentId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal RefundAmount { get; set; }
        public string? StaffUserId { get; set; }
    }

    public class RefundResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? GatewayReference { get; set; }
    }

    public class PenaltyPaymentDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = Domain.Enums.PaymentType.Penalty;
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int? ReservationId { get; set; }
        public long? SwapTransactionId { get; set; }
    }

    public class PaymentSummaryDto
    {
        public long PaymentId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? Description { get; set; }
        public long? SwapTransactionId { get; set; }
        public int? ReservationId { get; set; }
        public int? SubscriptionId { get; set; }

        public string RelatedEntity =>
            SwapTransactionId != null ? $"Swap #{SwapTransactionId}" :
            ReservationId != null ? $"Reservation #{ReservationId}" :
            SubscriptionId != null ? $"Subscription #{SubscriptionId}" : "-";
    }

    public class PaymentStatusResponseDto
    {
        public string? Code { get; set; }
        public string? Status { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentStatusUpdateDto
    {
        public string OrderCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? RawData { get; set; }
        public string? Signature { get; set; }
        public string? Reason { get; set; }
    }
}
