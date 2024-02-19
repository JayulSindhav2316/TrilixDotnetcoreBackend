using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Membershipperiod
    {
        public Membershipperiod()
        {
            Membershiptypes = new HashSet<Membershiptype>();
        }

        public int MembershipPeriodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PeriodUnit { get; set; }
        public int Duration { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Membershiptype> Membershiptypes { get; set; }
    }
}
