using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class CreditCardVoidModel
    {
        public int OrganizationId { get; set; }
        public int EntityId { get; set; }
        public int ReceiptId { get; set; }
        public string PaymentMode { get; set; }
        public decimal VoidAmount { get; set; }
        public string ReferenceTransactionId { get; set; }
    }
}
