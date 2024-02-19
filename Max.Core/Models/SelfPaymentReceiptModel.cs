using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class SelfPaymentReceiptModel
    {
        public string PaymentToken { get; set; }
        public int CartId { get; set; }

    }
}
