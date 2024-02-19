using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AuthNetChargePaymentProfileRequestModel
    {
        public AuthNetChargePaymentProfileRequestModel()
        {
            InvoiceDetails = new List<InvoiceDetailModel>();
        }
        public int OrganizationId { get; set; }
        public int PersonId { get; set; }
        public string ProfileId { get; set; }
        public string PaymentProfileId { get; set; }
        public string AuthNetPaymentProfileId { get; set; }
        public string MerchantLoginId { get; set; }
        public string TransactionKey { get; set; }
        public string Amount { get; set; }
        public List<InvoiceDetailModel> InvoiceDetails { get; set; }
        public int DraftId { get; set; }
        public int ReceiptId { get; set; }
        public int CartId { get; set; }
        public string AuthNetUrl { get; set; }
        public string PaymentMode { get; set; }
    }
}
