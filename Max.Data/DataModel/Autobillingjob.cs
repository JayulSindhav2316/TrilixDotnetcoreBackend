using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Autobillingjob
    {
        public int AutoBillingJobId { get; set; }
        public string JobType { get; set; }
        public string BillingType { get; set; }
        public string InvoiceType { get; set; }
        public DateTime ThroughDate { get; set; }
        public DateTime Create { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Status { get; set; }
        public int? AbpdId { get; set; }
    }
}
