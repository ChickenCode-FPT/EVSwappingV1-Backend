using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class UpdateBatteryDto
    {
        public int BatteryId { get; set; }

        public string SerialNumber { get; set; }

        public int BatteryModelId { get; set; }

        public decimal? CurrentSoH { get; set; }

        public int? CycleCount { get; set; }

        public string Status { get; set; }

        public DateTime? LastMaintenance { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
