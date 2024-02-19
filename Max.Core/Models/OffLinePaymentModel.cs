using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class OffLinePaymentModel
    {
        public int OrganizationId { get; set; }
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int EntityId { get; set; }
        public string PayerName { get; set; }
        public string PaymentType { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
