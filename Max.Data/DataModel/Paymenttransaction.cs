using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Paymenttransaction
    {
        public Paymenttransaction()
        {
            Autobillingpayments = new HashSet<Autobillingpayment>();
            Invoices = new HashSet<Invoice>();
        }

        public int PaymentTransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? EntityId { get; set; }
        public int? ReceiptId { get; set; }
        public decimal Amount { get; set; }
        public int? TransactionType { get; set; }
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
        public int? ShoppingCartId { get; set; }
        public string NickName { get; set; }
        public string OfflinePaymentType { get; set; }

        public virtual Entity Entity { get; set; }
        public virtual Receiptheader Receipt { get; set; }
        public virtual ICollection<Autobillingpayment> Autobillingpayments { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
