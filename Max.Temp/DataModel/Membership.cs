using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Membership
    {
        public Membership()
        {
            Invoices = new HashSet<Invoice>();
            Membershiphistories = new HashSet<Membershiphistory>();
        }

        public int MembershipId { get; set; }
        public int? MembershipTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ReviewDate { get; set; }
        public int? BillablePersonId { get; set; }
        public DateTime? NextBillDate { get; set; }
        public decimal? MaxDraftAmount { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public int? Status { get; set; }

        public virtual Person BillablePerson { get; set; }
        public virtual Membershiptype MembershipType { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Membershiphistory> Membershiphistories { get; set; }
    }
}
