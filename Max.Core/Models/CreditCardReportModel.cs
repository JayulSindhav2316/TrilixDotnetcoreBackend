using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class CreditCardReportModel
    {
        public int ReceiptId { get; set; }
        public string TransactionDate { get; set; }
        public string BillableName { get; set; }
        public string CreditCardNumber { get; set; }
        public string CardType { get; set; }
        public string PaymentStatus { get; set; }
        public string AuthCode { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }

    }
}
