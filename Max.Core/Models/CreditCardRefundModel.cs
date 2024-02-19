using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class CreditCardRefundModel
    {
        public int OrganizationId { get; set; }
        public int EntityId { get; set; }
        public int ReceiptId { get; set; }
        public int ReceiptDetailId { get; set; }
        public decimal RefundAmount { get; set; }
        public string RefundTransactionId { get; set; }
        public string CreditCardNumber { get; set; }
    }
}
