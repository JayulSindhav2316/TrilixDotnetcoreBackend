using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Organization
    {
        public Organization()
        {
            Accountingsetups = new HashSet<Accountingsetup>();
            Configurations = new HashSet<Configuration>();
            Departments = new HashSet<Department>();
            Documents = new HashSet<Document>();
            Entities = new HashSet<Entity>();
            Events = new HashSet<Event>();
            Paymentprocessors = new HashSet<Paymentprocessor>();
            People = new HashSet<Person>();
            Receiptheaders = new HashSet<Receiptheader>();
            Reports = new HashSet<Report>();
            Staffusers = new HashSet<Staffuser>();
        }

        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Prefix { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Logo { get; set; }
        public string Website { get; set; }
        public string HeaderImage { get; set; }
        public string FooterImge { get; set; }
        public string Email { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string TwilioAccountSid { get; set; }
        public string TwilioAuthToken { get; set; }
        public string PrintMessage { get; set; }
        public string WebMessage { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string LinkedIn { get; set; }
        public string Code { get; set; }
        public string AccountName { get; set; }
        public ulong? IsBirthdayRequired { get; set; }

        public virtual ICollection<Accountingsetup> Accountingsetups { get; set; }
        public virtual ICollection<Configuration> Configurations { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<Entity> Entities { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Paymentprocessor> Paymentprocessors { get; set; }
        public virtual ICollection<Person> People { get; set; }
        public virtual ICollection<Receiptheader> Receiptheaders { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<Staffuser> Staffusers { get; set; }
    }
}
