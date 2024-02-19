using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Staffuser
    {
        public Staffuser()
        {
            Autobillingonholds = new HashSet<Autobillingonhold>();
            Communications = new HashSet<Communication>();
            Contactactivities = new HashSet<Contactactivity>();
            Documentcontainers = new HashSet<Documentcontainer>();
            Documentobjectaccesshistories = new HashSet<Documentobjectaccesshistory>();
            Documentobjects = new HashSet<Documentobject>();
            Entityrolehistories = new HashSet<Entityrolehistory>();
            Eventcontacts = new HashSet<Eventcontact>();
            Journalentryheaders = new HashSet<Journalentryheader>();
            Membershiphistories = new HashSet<Membershiphistory>();
            //Membershipreports = new HashSet<Membershipreport>();
            Multifactorcodes = new HashSet<Multifactorcode>();
            Receiptheaders = new HashSet<Receiptheader>();
            Refunddetails = new HashSet<Refunddetail>();
            Reports = new HashSet<Report>();
            Shoppingcarts = new HashSet<Shoppingcart>();
            Staffroles = new HashSet<Staffrole>();
            Tokens = new HashSet<Token>();
            Userdevices = new HashSet<Userdevice>();
            Voiddetails = new HashSet<Voiddetail>();
            Writeoffs = new HashSet<Writeoff>();
            Opportunities = new HashSet<Opportunity>();
        }

        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public int DepartmentId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
        public DateTime? LastAccessed { get; set; }
        public string Salt { get; set; }
        public string CellPhoneNumber { get; set; }
        public int Locked { get; set; }
        public int FailedAttempts { get; set; }
        public int ProfilePictureId { get; set; }
        public int? SociableUserId { get; set; }
        public int? SociableProfileId { get; set; }
        public int? IsDisplayUser { get; set; }

        public virtual Department Department { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<Autobillingonhold> Autobillingonholds { get; set; }
        public virtual ICollection<Communication> Communications { get; set; }
        public virtual ICollection<Contactactivity> Contactactivities { get; set; }
        public virtual ICollection<Documentcontainer> Documentcontainers { get; set; }
        public virtual ICollection<Documentobjectaccesshistory> Documentobjectaccesshistories { get; set; }
        public virtual ICollection<Documentobject> Documentobjects { get; set; }
        public virtual ICollection<Entityrolehistory> Entityrolehistories { get; set; }
        public virtual ICollection<Eventcontact> Eventcontacts { get; set; }
        public virtual ICollection<Journalentryheader> Journalentryheaders { get; set; }
        public virtual ICollection<Membershiphistory> Membershiphistories { get; set; }
        public virtual ICollection<Multifactorcode> Multifactorcodes { get; set; }
        public virtual ICollection<Receiptheader> Receiptheaders { get; set; }
        public virtual ICollection<Refunddetail> Refunddetails { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<Shoppingcart> Shoppingcarts { get; set; }
        public virtual ICollection<Staffrole> Staffroles { get; set; }
        public virtual ICollection<Token> Tokens { get; set; }
        public virtual ICollection<Userdevice> Userdevices { get; set; }
        public virtual ICollection<Voiddetail> Voiddetails { get; set; }
        public virtual ICollection<Writeoff> Writeoffs { get; set; }
        public virtual ICollection<Opportunity> Opportunities { get; set; }
    }
}
