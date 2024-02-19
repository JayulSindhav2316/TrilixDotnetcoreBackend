using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Membershiptype
    {
        public Membershiptype()
        {
            Billingbatches = new HashSet<Billingbatch>();
            Containeraccesses = new HashSet<Containeraccess>();
            Documentaccesses = new HashSet<Documentaccess>();
            Membershipfees = new HashSet<Membershipfee>();
            Memberships = new HashSet<Membership>();
            Registrationgroupmembershiplinks = new HashSet<Registrationgroupmembershiplink>();
        }

        public int MembershipTypeId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? Period { get; set; }
        public int? PaymentFrequency { get; set; }
        public int? Category { get; set; }
        public int? Status { get; set; }
        public int Units { get; set; }

        public virtual Membershipcategory CategoryNavigation { get; set; }
        public virtual Membershipperiod PeriodNavigation { get; set; }
        public virtual ICollection<Billingbatch> Billingbatches { get; set; }
        public virtual ICollection<Containeraccess> Containeraccesses { get; set; }
        public virtual ICollection<Documentaccess> Documentaccesses { get; set; }
        public virtual ICollection<Membershipfee> Membershipfees { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; }
        public virtual ICollection<Registrationgroupmembershiplink> Registrationgroupmembershiplinks { get; set; }
    }
}
