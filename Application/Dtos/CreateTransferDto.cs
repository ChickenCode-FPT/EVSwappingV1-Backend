using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class CreateTransferDto
    {
        public int FromStationId { get; set; }
        public int ToStationId { get; set; }
        public int BatteryId { get; set; }
        public string RequestedByUserId { get; set; } = default!;
    }
}
