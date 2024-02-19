using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Max.Core.Models
{
    public class PersonProfileModel
    {
        public PersonProfileModel()
        {
            Emails = new List<EmailModel>();
            Phones = new List<PhoneModel>();
            Addresses = new List<AddressModel>();
            InvoicePayments = new List<InvoicePaymentModel>();
            Communications = new List<CommunicationModel>();
            Relations = new List<RelationModel>();
            PaymentProfiles = new List<PaymentProfileModel>();
            PaymentTransactions = new List<PaymentTransactionModel>();
            MembershipConnections = new List<MembershipConnectionModel>();
            Memberships = new List<MembershipModel>();
            CreditTransactions = new List<CreditTransactionModel>();
            Notes = new List<NotesModel>();
        }
        public int PersonId { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string Title { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? CompanyId { get; set; }
        public string TwitterName { get; set; }
        public string FacebookName { get; set; }
        public string LinkedinName { get; set; }
        public string SkypeName { get; set; }
        public string Salutation { get; set; }
        public int? Status { get; set; }
        public string Website { get; set; }
        public int? OrganizationId { get; set; }
        public  string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string PrimaryEmail { get; set; }
        public string PrimaryPhone { get; set; }
        public string MembershipStatus { get; set; }
        public string MembershipType { get; set; }
        public string JoinDate { get; set; }
        public string ExpirationDate { get; set; }
        public decimal CreditBalance { get; set; }
        public string NextBillDate { get; set; }
        public string WebLoginName { get; set; }
        public CompanyModel Company { get; set; }
        public List<InvoicePaymentModel> InvoicePayments { get; set; }
        public List<CommunicationModel> Communications { get; set; }
        public List<RelationModel> Relations { get; set; }
        public List<EmailModel> Emails { get; set; }
        public List<PhoneModel> Phones { get; set; }
        public List<AddressModel> Addresses { get; set; }
        public List<MembershipConnectionModel> MembershipConnections { get; set; }
        public List<MembershipModel> Memberships { get; set; }
        public List<PaymentProfileModel> PaymentProfiles { get; set; }
        public List<PaymentTransactionModel> PaymentTransactions { get; set; }
        public List<CreditTransactionModel> CreditTransactions { get; set; }
        public List<NotesModel> Notes { get; set; }
        public int? PreferredContact { get; set; }
        public int Age
        {
            get
            {
                if (DateOfBirth != null)
                {
                    return new DateTime(DateTime.Now.Subtract(DateOfBirth ?? DateTime.Now).Ticks).Year - 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public bool IsMember
        {
            get
            {
                if (MembershipConnections != null)
                {
                    return MembershipConnections.Where(x => x.Membership.Status == 1).Any();
                }
                else
                {
                    return false;
                }
            }
        }
        public string FormattedPhoneNumber
        {
            get
            {
                return PrimaryPhone.FormatPhoneNumber();
            }
        }

        public string FormattedZip
        {
            get
            {
                return Zip.FormatZip();
            }
        }
    }
}
