using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class BillingDocumentModel
    {
        public int BillingDocumentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string InvoiceType { get; set; }
        public string BillingType { get; set; }
        public int? IsFinalized { get; set; }
        public int Status { get; set; }
        public int? AbpdId { get; set; }
        public DateTime ThroughDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int InvoiceCount { get; set; }
        public decimal TotalAmount {get;set;}
    }
}
