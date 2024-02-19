using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReceiptModel
    {
        public ReceiptModel()
        {
            PaymentTransactions = new List<PaymentTransactionModel>();
            LineItems = new List<ReceiptLineItem>();
        }

        public int Receiptid { get; set; }
        public DateTime Date { get; set; }
        public int StaffId { get; set; }
        public string PaymentMode { get; set; }
        public int? PaymentTransactionId { get; set; }
        public string CheckNo { get; set; }
        public int Status { get; set; }
        public int OrganizationId { get; set; }
        public EntityModel BillableEntity { get; set; }
        public int? EntityId { get; set; }
        public BillingAddressModel BillingAddress { get; set; }
        public List<ReceiptLineItem> LineItems { get; set; }
        public OrganizationModel Organization { get; set; }
        public List<PaymentTransactionModel> PaymentTransactions { get; set; }
        public decimal CreditUsed { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDueAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal AmountExceptCreditUsed { get; set; }
        public string Notes { get; set; }
    }
}
