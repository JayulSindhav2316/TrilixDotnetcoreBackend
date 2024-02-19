using Max.Data.Audit;
using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    [Auditable]
    public partial class Paperinvoice
    {
        public int PaperInvoiceId { get; set; }
        public int PaperBillingCycleId { get; set; }
        public int InvoiceId { get; set; }
        public int? EntityId { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Entity Entity { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual Billingcycle PaperBillingCycle { get; set; }
    }
}
