using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class InterStationTransferAdminDto
    {
        public long TransferId { get; set; }
        public int FromStationId { get; set; }
        public string? FromStationName { get; set; }

        public int ToStationId { get; set; }
        public string? ToStationName { get; set; }

        public int BatteryId { get; set; }
        public string? BatterySerial { get; set; }

        public string? RequestedByUserId { get; set; }
        public string? RequestedBy { get; set; }

        public string? ApprovedByUserId { get; set; }
        public string? ApprovedBy { get; set; }

        public string Status { get; set; } = default!;
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public bool CanApprove => Status == "Pending";

    }
}
