using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Billingdocument
    {
        public Billingdocument()
        {
            Autobillingdrafts = new HashSet<Autobillingdraft>();
        }

        public int BillingDocumentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string InvoiceType { get; set; }
        public string BillingType { get; set; }
        public int? IsFinalized { get; set; }
        public int Status { get; set; }
        public int? AbpdId { get; set; }
        public DateTime ThroughDate { get; set; }

        public virtual ICollection<Autobillingdraft> Autobillingdrafts { get; set; }
    }
}
