using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class CreditTransactionModel
    {
        public int CreditTransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int PersonId { get; set; }
        public int ReceiptDetailId { get; set; }
        public decimal Amount { get; set; }
        public int EntryType { get; set; }
        public string CreditGlAccount { get; set; }
        public string DebitGlAccount { get; set; }
        public DateTime ExpirDate { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
    }
}
