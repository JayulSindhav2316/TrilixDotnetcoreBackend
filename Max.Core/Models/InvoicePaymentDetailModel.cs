using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class InvoicePaymentDetailModel
    {
        public InvoicePaymentDetailModel()
        {
            Payments = new List<PaymentDetailModel>();
        }
        public int InvoiceDetailId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public decimal Refund { get; set; }
        public decimal Discount { get; set; }
        public int FeeId { get; set; }
        public int ItemId { get; set; }
        public int ItemType { get; set; }
        public List<PaymentDetailModel> Payments { get; set; }
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
    }
}
