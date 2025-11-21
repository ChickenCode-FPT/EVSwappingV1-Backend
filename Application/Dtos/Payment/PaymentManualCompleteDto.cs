using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Payment
{
    public class PaymentManualCompleteDto
    {
        public long PaymentId { get; set; }
        public string Method { get; set; } = "Cash";
        public string StaffUserId { get; set; } = "";
        public DateTime? PaidAt { get; set; }
    }
}
