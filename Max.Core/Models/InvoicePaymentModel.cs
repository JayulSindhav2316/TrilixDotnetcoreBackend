using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class InvoicePaymentModel
    {
        public InvoicePaymentModel()
        {
            PaymentDetails = new List<PaymentDetailModel>();
            WriteOffDetails = new List<WriteOffModel>();
            InvoiceDetails  = new List<InvoiceDetailModel>();
        }
        public int EntityId { get; set; }
        public int InvoiceId { get; set; }
        public int InvoiceDetailId { get; set; }
        public int ReceiptId { get; set; }
        public string InvoiceType { get; set; }
        public int MembershipId { get; set; }
        public int MembershipTypeId { get; set; }
        public string MembershipType { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? DueDate { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public decimal WriteOff { get; set; }
        public decimal Refund { get; set; }
        public string Notes { get; set; }
        public int FeeId { get; set; }
        public int ItemId { get; set; }
        public int ItemType { get; set; }
        public string EntityName { get; set; }
        public string BillableEntityName { get; set; }
        public List<InvoiceDetailModel> InvoiceDetails { get; set; }
        public bool IsPaperInvoiceFinalized { get; set; } = true;
        public List<PaymentDetailModel> PaymentDetails { get; set; }
        public List<WriteOffModel> WriteOffDetails { get; set; }
        public int? EventId { get; set; }
        public bool HasBalance 
        { 
            get {
                if (Balance > 0)
                    return true;
                else
                    return false; 
            } 
        }
        public bool HasPayment
        {
            get
            {
                if (Paid > 0)
                    return true;
                else
                    return false;
            }
        }
        public bool HasRefund
        {
            get
            {
                if (Refund > 0)
                    return true;
                else
                    return false;
            }
        }
        public bool HasWriteOff
        {
            get
            {
                if (WriteOff > 0)
                    return true;
                else
                    return false;
            }
        }
    }
}
