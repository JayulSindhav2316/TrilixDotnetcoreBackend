using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class PaymentDetailModel
    {
        public int ReceiptId { get; set; }
        public int ReceiptDetailId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Refund { get; set; }
        public decimal Tax { get; set; }
        public int Status { get; set; }
        public string PaymentMode { get; set; }
        public int RefundDetailId { get; set; }

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

        public bool Voided
        {
            get
            {
                if (Status  == (int) ReceiptStatus.Void)
                    return true;
                else
                    return false;
            }
        }

        public bool CanVoid
        {
            get
            {
                if (PaymentDate.Date == DateTime.Now.Date && Status != (int)ReceiptStatus.Void)
                    return true;
                else
                    return false;
            }
        }

    }
}
