using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Entity
    {
        public Entity()
        {
            Answertoquestions = new HashSet<Answertoquestion>();
            Autobillingdrafts = new HashSet<Autobillingdraft>();
            Communications = new HashSet<Communication>();
            Companies = new HashSet<Company>();
            Contactactivities = new HashSet<Contactactivity>();
            Contactactivityinteractions = new HashSet<Contactactivityinteraction>();
            Credittransactions = new HashSet<Credittransaction>();
            Customfielddata = new HashSet<Customfielddatum>();
            Documentobjectaccesshistories = new HashSet<Documentobjectaccesshistory>();
            Entityrolehistories = new HashSet<Entityrolehistory>();
            Entityroles = new HashSet<Entityrole>();
            Eventregisters = new HashSet<Eventregister>();
            Groupmembers = new HashSet<Groupmember>();
            InvoiceBillableEntities = new HashSet<Invoice>();
            InvoiceEntities = new HashSet<Invoice>();
            Membershipconnections = new HashSet<Membershipconnection>();
            Memberships = new HashSet<Membership>();
            Multifactorcodes = new HashSet<Multifactorcode>();
            Notes = new HashSet<Note>();
            Paperinvoices = new HashSet<Paperinvoice>();
            Paymentprofiles = new HashSet<Paymentprofile>();
            Paymenttransactions = new HashSet<Paymenttransaction>();
            People = new HashSet<Person>();
            Receiptheaders = new HashSet<Receiptheader>();
            RelationEntities = new HashSet<Relation>();
            RelationRelatedEntities = new HashSet<Relation>();
            Sessionleaderlinks = new HashSet<Sessionleaderlink>();
            Userdevices = new HashSet<Userdevice>();
            Opportunities = new HashSet<Opportunity>();
        }

        public int EntityId { get; set; }
        public int? PersonId { get; set; }
        public int? CompanyId { get; set; }
        public string Name { get; set; }
        public int? OrganizationId { get; set; }
        public int? ProfilePictureId { get; set; }
        public string WebLoginName { get; set; }
        public string WebPassword { get; set; }
        public string WebPasswordSalt { get; set; }
        public int? PreferredBillingCommunication { get; set; }
        public int? AccountLocked { get; set; }
        public int? PasswordFailedAttempts { get; set; }
        public DateTime? PortalLastAccessed { get; set; }
        public int? SociableUserId { get; set; }
        public int? SociableProfileId { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<Answertoquestion> Answertoquestions { get; set; }
        public virtual ICollection<Autobillingdraft> Autobillingdrafts { get; set; }
        public virtual ICollection<Communication> Communications { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<Contactactivity> Contactactivities { get; set; }
        public virtual ICollection<Contactactivityinteraction> Contactactivityinteractions { get; set; }
        public virtual ICollection<Credittransaction> Credittransactions { get; set; }
        public virtual ICollection<Customfielddatum> Customfielddata { get; set; }
        public virtual ICollection<Documentobjectaccesshistory> Documentobjectaccesshistories { get; set; }
        public virtual ICollection<Entityrolehistory> Entityrolehistories { get; set; }
        public virtual ICollection<Entityrole> Entityroles { get; set; }
        public virtual ICollection<Eventregister> Eventregisters { get; set; }
        public virtual ICollection<Groupmember> Groupmembers { get; set; }
        public virtual ICollection<Invoice> InvoiceBillableEntities { get; set; }
        public virtual ICollection<Invoice> InvoiceEntities { get; set; }
        public virtual ICollection<Membershipconnection> Membershipconnections { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; }
        public virtual ICollection<Multifactorcode> Multifactorcodes { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Paperinvoice> Paperinvoices { get; set; }
        public virtual ICollection<Paymentprofile> Paymentprofiles { get; set; }
        public virtual ICollection<Paymenttransaction> Paymenttransactions { get; set; }
        public virtual ICollection<Person> People { get; set; }
        public virtual ICollection<Receiptheader> Receiptheaders { get; set; }
        public virtual ICollection<Relation> RelationEntities { get; set; }
        public virtual ICollection<Relation> RelationRelatedEntities { get; set; }
        public virtual ICollection<Sessionleaderlink> Sessionleaderlinks { get; set; }
        public virtual ICollection<Userdevice> Userdevices { get; set; }
        public virtual ICollection<Opportunity> Opportunities { get; set; }

    }
}
