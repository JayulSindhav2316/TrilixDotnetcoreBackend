using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{

    public partial class Membership
    {
        public Membership()
        {
            Autobillingdrafts = new HashSet<Autobillingdraft>();
            Autobillingonholds = new HashSet<Autobillingonhold>();
            Billingfees = new HashSet<Billingfee>();
            Invoices = new HashSet<Invoice>();
            Membershipconnections = new HashSet<Membershipconnection>();
            Membershiphistories = new HashSet<Membershiphistory>();
            Shoppingcartdetails = new HashSet<Shoppingcartdetail>();
        }

        public int MembershipId { get; set; }
        public int? MembershipTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ReviewDate { get; set; }
        public int? BillableEntityId { get; set; }
        public DateTime NextBillDate { get; set; }
        public int BillingOnHold { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime RenewalDate { get; set; }
        public DateTime TerminationDate { get; set; }
        public int Status { get; set; }
        public int AutoPayEnabled { get; set; }
        public string PaymentProfileId { get; set; }

        public virtual Entity BillableEntity { get; set; }
        public virtual Membershiptype MembershipType { get; set; }
        public virtual ICollection<Autobillingdraft> Autobillingdrafts { get; set; }
        public virtual ICollection<Autobillingonhold> Autobillingonholds { get; set; }
        public virtual ICollection<Billingfee> Billingfees { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Membershipconnection> Membershipconnections { get; set; }
        public virtual ICollection<Membershiphistory> Membershiphistories { get; set; }
        public virtual ICollection<Shoppingcartdetail> Shoppingcartdetails { get; set; }
    }
}
