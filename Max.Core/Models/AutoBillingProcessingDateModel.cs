using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AutoBillingProcessingDateModel
    {
        public int AutoBillingProcessingDatesId { get; set; }
        public string BillingType { get; set; }
        public string InvoiceType { get; set; }
        public DateTime ThroughDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int? Status { get; set; }
        public int? IsLastDayOfThroughDate { get; set; }
        public int? IsLastDayOfEffectiveDate { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
