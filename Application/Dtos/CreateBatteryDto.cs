using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class CreateBatteryDto
    {
        public int ModelId { get; set; }
        public string Status { get; set; }
        public int Capacity { get; set; }
    }

}
