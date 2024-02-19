using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Credittransaction
    {
        public int CreditTransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int EntityId { get; set; }
        public int ReceiptDetailId { get; set; }
        public int EntryType { get; set; }
        public string CreditGlAccount { get; set; }
        public string DebitGlAccount { get; set; }
        public DateTime ExpirDate { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public decimal? Amount { get; set; }

        public virtual Entity Entity { get; set; }
        public virtual Receiptdetail ReceiptDetail { get; set; }
    }
}
