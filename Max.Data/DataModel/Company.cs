using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Company
    {
        public Company()
        {
            Addresses = new HashSet<Address>();
            Contactactivities = new HashSet<Contactactivity>();
            Contactactivityinteractions = new HashSet<Contactactivityinteraction>();
            Emails = new HashSet<Email>();
            Entityrolehistories = new HashSet<Entityrolehistory>();
            Entityroles = new HashSet<Entityrole>();
            People = new HashSet<Person>();
            Phones = new HashSet<Phone>();
            Opportunities = new HashSet<Opportunity>();
        }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public int? BillableContactId { get; set; }
        public int? EntityId { get; set; }
        public string Country { get; set; }
        public string MemberId { get; set; }

        public virtual Entity Entity { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Contactactivity> Contactactivities { get; set; }
        public virtual ICollection<Contactactivityinteraction> Contactactivityinteractions { get; set; }
        public virtual ICollection<Email> Emails { get; set; }
        public virtual ICollection<Entityrolehistory> Entityrolehistories { get; set; }
        public virtual ICollection<Entityrole> Entityroles { get; set; }
        public virtual ICollection<Person> People { get; set; }
        public virtual ICollection<Phone> Phones { get; set; }
        public virtual ICollection<Opportunity> Opportunities { get; set; }
    }
}
