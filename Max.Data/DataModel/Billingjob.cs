using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Billingjob
    {
        public int BillingJobId { get; set; }
        public int BillingCycleId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }

        public virtual Billingcycle BillingCycle { get; set; }
    }
}
