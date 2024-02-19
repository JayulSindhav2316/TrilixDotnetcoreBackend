using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class BillingCycleModel
    {
        public BillingCycleModel()
        {
            BillingJobs = new List<BillingJobModel>();
            PaperInvoices = new List<PaperInvoiceModel>();
            BillingBatches = new List<BillingBatchModel>();
        }

        public int BillingCycleId { get; set; }
        public string CycleName { get; set; }
        public string[] MembershipType { get; set; }
        public DateTime RunDate { get; set; }
        public DateTime ThroughDate { get; set; }
        public int Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<BillingJobModel> BillingJobs { get; set; }
        public List<PaperInvoiceModel> PaperInvoices { get; set; }
        public int InvoiceCount { get; set; }
        public List<BillingBatchModel> BillingBatches { get; set; }
        public int CycleType { get; set; }


    }
}
