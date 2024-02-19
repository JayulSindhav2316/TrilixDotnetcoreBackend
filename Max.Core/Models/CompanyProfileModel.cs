using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Max.Core.Models
{
    public class CompanyProfileModel
    {
        public CompanyProfileModel()
        {
            
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
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public int BillableContactId { get; set; }
        public string MailingAddress { get; set; }
        public PersonModel BillablePerson { get; set; }
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
        public List<MembershipConnectionModel> MembershipConnections { get; set; }
        public List<MembershipModel> Memberships { get; set; }
        public List<PaymentProfileModel> PaymentProfiles { get; set; }
        public List<PaymentTransactionModel> PaymentTransactions { get; set; }
        public List<CreditTransactionModel> CreditTransactions { get; set; }
        public List<NotesModel> Notes { get; set; }
        
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
                return Phone.FormatPhoneNumber();
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
