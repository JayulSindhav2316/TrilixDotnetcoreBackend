using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Membershiptype
    {
        public Membershiptype()
        {
            Membershipfees = new HashSet<Membershipfee>();
            Memberships = new HashSet<Membership>();
        }

        public int MembershipTypeId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? Period { get; set; }
        public int? Category { get; set; }
        public int? Status { get; set; }

        public virtual Membershipcategory CategoryNavigation { get; set; }
        public virtual Membershipperiod PeriodNavigation { get; set; }
        public virtual ICollection<Membershipfee> Membershipfees { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; }
    }
}
