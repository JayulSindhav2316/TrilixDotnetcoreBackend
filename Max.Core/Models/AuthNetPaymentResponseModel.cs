using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AuthNetPaymentResponseModel
    {
        public string TransactionId { get; set; }
        public string ResponseCode { get; set; }
        public string AuthCode { get; set; }
        public string MessageCode { get; set; }
        public string MessageDescription { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public int  TransactionStatus { get; set; }

    }
}
