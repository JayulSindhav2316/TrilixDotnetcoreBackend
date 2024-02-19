using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class PaymentTransactionModel
    {
        public int PaymentTransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int ShoppingCartId { get; set; }
        public int? EntityId { get; set; }
        public int? ReceiptId { get; set; }
        public decimal? Amount { get; set; }
        public int TransactionType { get; set; }
        public string PaymentType { get; set; }
        public string CardType { get; set; }
        public string CreditCardHolderName { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public string BankName { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountType { get; set; }
        public string RefId { get; set; }
        public string AuthCode { get; set; }
        public string ResponseCode { get; set; }
        public string MessageDetails { get; set; }
        public int? Status { get; set; }
        public int? Result { get; set; }
        public int? IsAdjusted { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReferenceTransactionId { get; set; }
        public string TransactionId { get; set; }
        public string ResponseDetails { get; set; }
        public int AutoBillingDraftId { get; set; }
        public decimal CreditBalanceUsed { get; set; }
        public bool IsOfflinePayment { get; set; }
        public string OfflinePaymentType { get; set; }
        public string NickName { get; set; }
    }
}
