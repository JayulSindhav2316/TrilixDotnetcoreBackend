using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Autobillingonhold
    {
        public int AutoBillingOnHoldId { get; set; }
        public int MembershipId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Reason { get; set; }
        public DateTime? ReviewDate { get; set; }
        public int UserId { get; set; }

        public virtual Membership Membership { get; set; }
        public virtual Staffuser User { get; set; }
    }
}
