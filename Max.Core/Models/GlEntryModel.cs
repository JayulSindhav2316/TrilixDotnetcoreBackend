using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GlEntryModel
    {
        public int GlEntryId { get; set; }
        public DateTime EntryDate { get; set; }
        public int GlAccountId { get; set; }
        public string GlaccountCode { get; set; }
        public int ReceiptDetailId { get; set; }
        public string Description { get; set; }
        public string EntryType { get; set; }
        public string PaymentMode { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public int PaymentTransactionId { get; set; }

        public virtual GlAccountModel GlAccount { get; set; }
        public virtual PaymentTransactionModel PaymentTransaction { get; set; }
        public virtual ReceiptDetailModel ReceiptDetail { get; set; }
    }
}
