using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class BillingBatchModel
    {
        public int BillingBatchId { get; set; }
        public int BillingCycleId { get; set; }
        public string MembershipType { get; set; }
        public string Category { get; set; }
        public string Period { get; set; }
        public string Frequency { get; set; }
    }
}
