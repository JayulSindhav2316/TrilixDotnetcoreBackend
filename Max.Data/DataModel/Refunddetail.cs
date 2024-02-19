using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Refunddetail
    {
        public int RefundId { get; set; }
        public DateTime RefundDate { get; set; }
        public int RefundMode { get; set; }
        public int ReceiptDetailId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public decimal? ProcessingFee { get; set; }
        public int UserId { get; set; }
        public int PaymentTransactionId { get; set; }
        public int EntityId { get; set; }

        public virtual Receiptdetail ReceiptDetail { get; set; }
        public virtual Staffuser User { get; set; }
    }
}
