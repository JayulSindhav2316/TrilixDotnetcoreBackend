using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReceiptDetailModel
    {
        public int ReceiptDetailId { get; set; }
        public int ReceiptHeaderId { get; set; }
        public int PersonId { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public int MembershipFeeId { get; set; }
        public int InvoiceId { get; set; }
        public int InvoiceDetailId { get; set; }
        public decimal Tax { get; set; }
        public int PastDueInvoiceDetailRef { get; set; }
        public int ItemType { get; set; }
        public int RefundDetailId { get; set; }
        
    }
}
