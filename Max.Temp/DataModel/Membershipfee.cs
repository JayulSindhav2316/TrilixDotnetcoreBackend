using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Membershipfee
    {
        public int FeeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? FeeAmount { get; set; }
        public int? GlAccount { get; set; }
        public int? BillingFrequency { get; set; }
        public int? IsMandatory { get; set; }
        public int? Status { get; set; }
        public int? MembershipTypeId { get; set; }

        public virtual Glaccount GlAccountNavigation { get; set; }
        public virtual Membershiptype MembershipType { get; set; }
    }
}
