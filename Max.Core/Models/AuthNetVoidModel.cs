using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AuthNetVoidModel
    {
        public int PaymentTransactionId { get; set; }
        public bool IsPaymentAlreadyVoided { get; set; }
        public string PaymentVoidMessage { get; set; }
    }
}
