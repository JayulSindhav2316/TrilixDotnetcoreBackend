using System;
using System.Collections.Generic;
using System.Text;
using Max.Core.Models;

namespace Max.Core.Models
{
    public class ReceiptHeaderModel
    {
        public ReceiptHeaderModel()
        {
            ReceiptDetailModel = new List<ReceiptDetailModel>();
            PaymentTransactionModel = new List<PaymentTransactionModel>();
        }

        public int Receiptid { get; set; }
        public DateTime Date { get; set; }
        public int StaffId { get; set; }
        public string PaymentMode { get; set; }
        public int? PaymentTransactionId { get; set; }
        public string CheckNo { get; set; }
        public int Status { get; set; }
        public int? OrganizationId { get; set; }
        public List<ReceiptDetailModel> ReceiptDetailModel { get; set; }
        public List<PaymentTransactionModel> PaymentTransactionModel { get; set; }
        public string Notes { get; set; }
        public int PromoCodeId { get; set; }
        public int BillableEntityId { get; set; }

    }
}
