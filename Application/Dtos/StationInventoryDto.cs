using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class StationInventoryDto
    {
        public int StationId { get; set; }
        public int FullBatteries { get; set; }
        public int ChargingBatteries { get; set; }
        public int MaintenanceBatteries { get; set; }
        public List<BatteryDetailDto> Batteries { get; set; } = new();
    }
}
