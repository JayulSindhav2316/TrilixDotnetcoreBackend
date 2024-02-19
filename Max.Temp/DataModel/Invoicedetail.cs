using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Invoicedetail
    {
        public int InvoiceDetailId { get; set; }
        public int? InvoiceId { get; set; }
        public string Description { get; set; }
        public string GlAccount { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? Taxable { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? Amount { get; set; }
        public int? Status { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
