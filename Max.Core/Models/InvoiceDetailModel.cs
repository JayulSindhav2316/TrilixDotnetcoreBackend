using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class InvoiceDetailModel
    {
        public int InvoiceDetailId { get; set; }
        public int InvoiceId { get; set; }
        public string Description { get; set; }
        public string GlAccount { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int? Taxable { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public int Status { get; set; }
        public int FeeId { get; set; }
        public int ItemId { get; set; }
        public int ItemType { get; set; }
        public int GlAccountId { get; set; }
        public int? BillableEntityId { get; set; }

    }
}