using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class BatteryDetailDto
    {
        public int BatteryId { get; set; }
        public string Model { get; set; } = string.Empty;
        public decimal Capacity { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
