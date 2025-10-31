using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class GetInterStationTransferDto
    {
        public long TransferId { get; set; }
        public int FromStationId { get; set; }
        public string FromStationName { get; set; } = string.Empty;
        public int ToStationId { get; set; }
        public string ToStationName { get; set; } = string.Empty;
        public int BatteryId { get; set; }
        public string BatterySerial { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
    }
}
