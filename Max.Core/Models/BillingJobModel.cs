using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class BillingJobModel
    {
        public int BillingJobId { get; set; }
        public int BillingCycleId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public string BaseUrl { get; set; }
    }
}
