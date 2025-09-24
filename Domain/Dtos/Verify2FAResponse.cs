using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos
{
    public class Verify2FAResponse
    {
        public string Token { get; set; }          
        public string RefreshToken { get; set; }   
        public DateTime? RefreshTokenExpiry { get; set; }
    }

}
