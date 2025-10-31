using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class AssignStaffDto
    {
        public string UserId { get; set; } = null!;
        public string Role { get; set; } = "Staff";
    }
}
