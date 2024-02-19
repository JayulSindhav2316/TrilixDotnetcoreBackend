using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Membershiphistory
    {
        public int MembershipHistoryId { get; set; }
        public int? MembershipId { get; set; }
        public DateTime? StatusDate { get; set; }
        public int? Status { get; set; }
        public int? ChangedBy { get; set; }
        public string Reason { get; set; }
        public int? PreviousMembershipId { get; set; }

        public virtual Staffuser ChangedByNavigation { get; set; }
        public virtual Membership Membership { get; set; }
    }
}
