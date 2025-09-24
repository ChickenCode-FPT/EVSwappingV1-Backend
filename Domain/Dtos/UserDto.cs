using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos
{
    public class UserDto
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public List<string> Roles { get; set; } = new List<string>();
        public bool Lockout { get; set; }
    }
}
