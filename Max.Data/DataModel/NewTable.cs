using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class NewTable
    {
        public int AutoBillingPaymentId { get; set; }
        public int AutoBillingDraftId { get; set; }
        public int PaymentTransactionId { get; set; }
        public int ReceiptId { get; set; }
        public int Status { get; set; }

        public virtual Autobillingdraft AutoBillingDraft { get; set; }
        public virtual Paymenttransaction PaymentTransaction { get; set; }
        public virtual Receiptheader Receipt { get; set; }
    }
}
