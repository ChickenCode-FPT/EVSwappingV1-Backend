using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class PaymentDto
    {
        public long PaymentId { get; set; }
        public long? SwapTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string? Method { get; set; }
        public string Status { get; set; }// = "Pending";
        public DateTime? PaidAt { get; set; }
    }

    public class PaymentAndTranDto
    {
        public long PaymentId { get; set; }
        public long? SwapTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string? Method { get; set; }
        public string Status { get; set; }// = "Pending";
        public DateTime? PaidAt { get; set; }

        public TranscationDto Transcation { get; set; }

    }

    public class TranscationDto 
    {
        public long SwapTransactionId { get; set; }

        public int? ReservationId { get; set; }

        public int StationId { get; set; }

        public string CustomerUserId { get; set; }

        public string? StaffUserId { get; set; }

        public int? OutgoingBatteryId { get; set; }

        public int? IncomingBatteryId { get; set; }

        public DateTime SwapStartedAt { get; set; }

        public DateTime? SwapFinishedAt { get; set; }

        public string SwapStatus { get; set; }

        public decimal Price { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
