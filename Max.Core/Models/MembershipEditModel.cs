using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipEditModel
    {
        public int MembershipId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime NextBillDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BillableEntityId { get; set; }
        public string BillingNotificationPreference { get; set; }
        public int[] Members { get; set; }
    }
}
