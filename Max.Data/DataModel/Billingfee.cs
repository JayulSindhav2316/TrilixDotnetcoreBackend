using Max.Data.Audit;
using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    [Auditable]
    public partial class Billingfee
    {
        public int BillingFeeId { get; set; }
        public int MembershipId { get; set; }
        public int MembershipFeeId { get; set; }
        public decimal Fee { get; set; }
        public decimal Discount { get; set; }
        public int Status { get; set; }

        public virtual Membership Membership { get; set; }
        public virtual Membershipfee MembershipFee { get; set; }
    }
}
