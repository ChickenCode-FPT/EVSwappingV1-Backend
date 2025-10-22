using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public static class ReservationAllocationStatus
    {
        public const string Active = "Active";
        public const string Released = "Released";
        public const string Expired = "Expired";
        public const string Consumed = "Consumed";
    }
}
