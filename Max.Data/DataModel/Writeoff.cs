using Max.Data.Audit;
using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    [Auditable]
    public partial class Writeoff
    {
        public int WriteOffId { get; set; }
        public DateTime? Date { get; set; }
        public int? InvoiceDetailId { get; set; }
        public string Reason { get; set; }
        public decimal? Amount { get; set; }
        public int? UserId { get; set; }

        public virtual Invoicedetail InvoiceDetail { get; set; }
        public virtual Staffuser User { get; set; }
    }
}
