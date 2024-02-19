
using Max.Data.Audit;
using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    [Auditable]
    public partial class Invoicedetail
    {
        public Invoicedetail()
        {
            Receiptdetails = new HashSet<Receiptdetail>();
            Writeoffs = new HashSet<Writeoff>();
        }

        public int InvoiceDetailId { get; set; }
        public int InvoiceId { get; set; }
        public string Description { get; set; }
        public string GlAccount { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int? Taxable { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public int? FeeId { get; set; }
        public int? ItemId { get; set; }
        public int ItemType { get; set; }
        public int? BillableEntityId { get; set; }

        public virtual Membershipfee Fee { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<Receiptdetail> Receiptdetails { get; set; }
        public virtual ICollection<Writeoff> Writeoffs { get; set; }
    }
}
