using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class VoidRequestModel
    {
        public int UserId { get; set; }
        public int ReceiptId { get; set; }
        public string Reason { get; set; }
        public decimal VoidAmount { get; set; }
        public int VoidPaymentTransactionId { get; set; }
    }
}
