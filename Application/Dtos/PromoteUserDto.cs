using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class PromoteUserDto
    {
        public string NewRole { get; set; } = "Staff";
        public bool ReplaceExistingRoles { get; set; } = true;
    }
}
