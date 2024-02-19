using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GeneralLedgerModel
    {
        public int ReceiptId { get; set; }
        public string TransactionDate { get; set; }
        public string ItemDescription { get; set; }
        public string GlAccount { get; set; }
        public string PaymentMode { get; set; }
        public string TransactionType { get; set; }
        public string EntryType { get; set; }
        public decimal Amount { get; set; }
    }
}
