using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class StationStaffDto
    {
        public int StationStaffId { get; set; }
        public int StationId { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string Role { get; set; } = default!;
        public bool IsActive { get; set; }
    }

}
