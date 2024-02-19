using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Person
    {
        public Person()
        {
            Addresses = new HashSet<Address>();
            Emails = new HashSet<Email>();
            Persontags = new HashSet<Persontag>();
            Phones = new HashSet<Phone>();
        }

        public int PersonId { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string CasualName { get; set; }
        public string Suffix { get; set; }
        public string Gender { get; set; }
        public string Title { get; set; }
        public string Salutation { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? CompanyId { get; set; }
        public string FacebookName { get; set; }
        public string InstagramName { get; set; }
        public string LinkedinName { get; set; }
        public string SkypeName { get; set; }
        public string TwitterName { get; set; }
        public string Website { get; set; }
        public int? OrganizationId { get; set; }
        public int? Status { get; set; }
        public int? PreferredContact { get; set; }
        public int? EntityId { get; set; }
        public string Designation { get; set; }
        public string MemberId { get; set; }

        public virtual Company Company { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Email> Emails { get; set; }
        public virtual ICollection<Persontag> Persontags { get; set; }
        public virtual ICollection<Phone> Phones { get; set; }
    }
}
