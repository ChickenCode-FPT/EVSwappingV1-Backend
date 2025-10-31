using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class CreateBatteryHealthLogDto
    {
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime RecordedAt { get; set; }
        public decimal SoH { get; set; }
        public int CycleCount { get; set; }
        public decimal Temperature { get; set; }
        public string? Notes { get; set; }
    }
}
