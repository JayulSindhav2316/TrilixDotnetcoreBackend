using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class RefundDetailModel
    {
        public int RefundId { get; set; }
        public DateTime RefundDate { get; set; }
        public int RefundMode { get; set; }
        public int ReceiptId { get; set; }
        public int ReceiptDetailId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public decimal? ProcessingFee { get; set; }
        public int UserId { get; set; }
        public int PaymentTransactionId { get; set; }
        public int PersonId { get; set; }

    }
}
