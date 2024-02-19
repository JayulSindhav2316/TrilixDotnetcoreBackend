using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Receiptdetail
    {
        public Receiptdetail()
        {
            Credittransactions = new HashSet<Credittransaction>();
            Journalentrydetails = new HashSet<Journalentrydetail>();
            Receiptitemdetails = new HashSet<Receiptitemdetail>();
            Refunddetails = new HashSet<Refunddetail>();
        }

        public int ReceiptDetailId { get; set; }
        public int? ReceiptHeaderId { get; set; }
        public int? EntityId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public int? InvoiceDetailId { get; set; }
        public decimal? Tax { get; set; }
        public decimal Discount { get; set; }
        public int? PastDueInvoiceDetailRef { get; set; }
        public int? ItemType { get; set; }

        public virtual Invoicedetail InvoiceDetail { get; set; }
        public virtual Receiptheader ReceiptHeader { get; set; }
        public virtual ICollection<Credittransaction> Credittransactions { get; set; }
        public virtual ICollection<Journalentrydetail> Journalentrydetails { get; set; }
        public virtual ICollection<Receiptitemdetail> Receiptitemdetails { get; set; }
        public virtual ICollection<Refunddetail> Refunddetails { get; set; }
    }
}
