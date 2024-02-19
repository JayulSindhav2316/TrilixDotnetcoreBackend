using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AutoBillingHoldRequestModel
    {
        public int MembershipId { get; set; }
        public string Reason { get; set; }
        public DateTime? ReviewDate { get; set; }
        public int UserId { get; set; }
    }
}
