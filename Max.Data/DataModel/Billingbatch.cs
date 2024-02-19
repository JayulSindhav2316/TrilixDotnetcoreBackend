using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Billingbatch
    {
        public int BillingBatchId { get; set; }
        public int BatchCycleId { get; set; }
        public int MembershipTypeId { get; set; }
        public int Status { get; set; }

        public virtual Billingcycle BatchCycle { get; set; }
        public virtual Membershiptype MembershipType { get; set; }
    }
}
