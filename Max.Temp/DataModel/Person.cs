using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Person
    {
        public Person()
        {
            Addresses = new HashSet<Address>();
            Communications = new HashSet<Communication>();
            Documents = new HashSet<Document>();
            Emails = new HashSet<Email>();
            Invoices = new HashSet<Invoice>();
            Memberships = new HashSet<Membership>();
            Persontags = new HashSet<Persontag>();
            Phones = new HashSet<Phone>();
            RelationPeople = new HashSet<Relation>();
            RelationRelatedPeople = new HashSet<Relation>();
        }

        public int PersonId { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string Gender { get; set; }
        public string Title { get; set; }
        public string Salutation { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? CompanyId { get; set; }
        public string FacebookName { get; set; }
        public string LinkedinName { get; set; }
        public string SkypeName { get; set; }
        public string TwitterName { get; set; }
        public string Website { get; set; }
        public int? OrganizationId { get; set; }
        public int? MembershipId { get; set; }
        public int? ProfilePictureId { get; set; }
        public int? Status { get; set; }

        public virtual Company Company { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Communication> Communications { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<Email> Emails { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; }
        public virtual ICollection<Persontag> Persontags { get; set; }
        public virtual ICollection<Phone> Phones { get; set; }
        public virtual ICollection<Relation> RelationPeople { get; set; }
        public virtual ICollection<Relation> RelationRelatedPeople { get; set; }
    }
}
