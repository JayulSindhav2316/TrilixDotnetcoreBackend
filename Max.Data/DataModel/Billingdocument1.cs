using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Billingdocument1
    {
        public int BillingDocumentId { get; set; }
        public string DisplayName { get; set; }
        public string FilePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string DisplayPath { get; set; }
        public string InvoiceType { get; set; }
        public string BillingType { get; set; }
        public int? IsFinalized { get; set; }
        public string Info { get; set; }
        public int? Status { get; set; }
        public string DraftType { get; set; }
        public int? DocumentType { get; set; }
        public int? AbpdId { get; set; }
        public DateTime? ThroughDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
