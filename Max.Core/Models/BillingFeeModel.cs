using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class BillingFeeModel
    {
        public int BillingFeeId { get; set; }
        public int MembershipId { get; set; }
        public int MembershipFeeId { get; set; }
        public decimal Fee { get; set; }
        public int Status { get; set; }
        public MembershipFeeModel MembershipFee { get; set; }
    }
}
