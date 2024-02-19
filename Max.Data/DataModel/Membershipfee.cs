using Max.Data.Audit;
using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    [Auditable]
    public partial class Membershipfee
    {
        public Membershipfee()
        {
            Billingfees = new HashSet<Billingfee>();
            Invoicedetails = new HashSet<Invoicedetail>();
        }

        public int FeeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal FeeAmount { get; set; }
        public int? GlAccountId { get; set; }
        public int? BillingFrequency { get; set; }
        public int? IsMandatory { get; set; }
        public int Status { get; set; }
        public int? MembershipTypeId { get; set; }

        public virtual Glaccount GlAccount { get; set; }
        public virtual Membershiptype MembershipType { get; set; }
        public virtual ICollection<Billingfee> Billingfees { get; set; }
        public virtual ICollection<Invoicedetail> Invoicedetails { get; set; }
    }
}
