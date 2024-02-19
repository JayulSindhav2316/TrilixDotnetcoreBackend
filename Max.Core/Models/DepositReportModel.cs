using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class DepositReportModel
    {
        public int ReceiptId { get; set; }
        public string TransactionDate { get; set; }
        public string UserName { get; set; }
        public string BillableName { get; set; }
        public string PaymentMode { get; set; }
        public string CreditCardNumber { get; set; }
        public string CardType { get; set; }
        public string AuthCode { get; set; }
        public string BankName { get; set; }
        public string CheckNumber { get; set; }
        public string PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public string Portal { get; set; }
        public string TransactionReference { get; set; }
        public bool IsSummary { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalCheck { get; set; }
        public decimal TotalCreditCard { get; set; }
        public decimal TotalECheck { get; set; }
        public decimal TotalOffline { get; set; }
    }
}
