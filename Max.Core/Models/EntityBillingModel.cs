using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EntityBillingModel
    {
        public int EntityId { get; set; }
        public int MembershipId { get; set; }
        public string MembershipName { get; set; }
        public string PaymentMethod { get; set; }
        public string BillingFrequency { get; set; }
        public decimal Amount { get; set; }
        public int BillingOnHold { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime NextBillDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
