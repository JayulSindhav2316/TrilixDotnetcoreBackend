using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class SelfPaymentReceiptResponseModel
    {
        public string AuthorizationCode { get; set; }
        public string PaymentResponse { get; set; }
        public string TransactionId { get; set; }
        public ReceiptModel Receipt { get; set; }
    }
}
