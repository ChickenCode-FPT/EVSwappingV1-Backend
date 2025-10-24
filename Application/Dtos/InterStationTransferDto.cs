using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class InterStationTransferDto
    {
        public long TransferId { get; set; }
        public int FromStationId { get; set; }
        public string? FromStationName { get; set; }

        public int ToStationId { get; set; }
        public string? ToStationName { get; set; }

        public int BatteryId { get; set; }
        public string? BatteryCode { get; set; }

        public string? RequestedByUserId { get; set; }
        public string? RequestedByUserName { get; set; }

        public string? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }

        public string Status { get; set; } = default!;
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
