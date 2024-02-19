using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class RefundRequestModel
    {
        public int UserId { get; set; }
        public int ReceiptId { get; set; }
        public int InvoiceDetailId { get; set; }
        public int ReceiptDetailId { get; set; }
        public string Reason { get; set; }
        public decimal ProcessingFee { get; set; }
        public decimal RefundAmount { get; set; }
        public int RefundMode { get; set; }
        public int RefundPaymentTransactionId { get; set; }
    }
}
