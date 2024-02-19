using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace Max.Core.Models
{
    public  class EntityModel
    {
        public EntityModel()
        {
            Communications = new List<CommunicationModel>();
            CreditTransactions = new List<CreditTransactionModel>();
            MembershipConnections = new List<MembershipConnectionModel>();
            Memberships = new List<MembershipModel>();
            Notes = new List<NotesModel>();
            PaperInvoices = new List<PaperInvoiceModel>();
            PaymentProfiles = new List<PaymentProfileModel>();
            PaymentTransactions = new List<PaymentTransactionModel>();
            RelationEntities = new List<RelationModel>();
            RelationRelatedEntities = new List<RelationModel>();
            InvoicePayments = new List<InvoicePaymentModel>();
            EntityRoles = new List<EntityRoleModel>();
        }

        public int EntityId { get; set; }
        public int? PersonId { get; set; }
        public int? CompanyId { get; set; }
        public string Name { get; set; }
        public int? OrganizationId { get; set; }
        public int? ProfilePictureId { get; set; }
        public string MembershipStatus { get; set; }
        public string JoinDate { get; set; }
        public string ExpirationDate { get; set; }
        public decimal CreditBalance { get; set; }
        public string NextBillDate { get; set; }
        public string WebLoginName { get; set; }
        public string WebPassword { get; set; }
        public string WebPasswordSalt { get; set; }
        public int AccountLocked { get; set; }
        public int PreferredBillingCommunication { get; set; }
        public int? SociableUserId { get; set; }
        public int? SociableProfileId { get; set; }
        public  CompanyModel Company { get; set; }
        public  OrganizationModel Organization { get; set; }
        public  PersonModel Person { get; set; }
        public  List<CommunicationModel> Communications { get; set; }
        public  List<CreditTransactionModel> CreditTransactions { get; set; }
        public  List<InvoiceModel> Invoices { get; set; }
        public List<InvoicePaymentModel> InvoicePayments { get; set; }
        public List<MembershipConnectionModel> MembershipConnections { get; set; }
        public List<MembershipModel> Memberships { get; set; }
        public List<NotesModel> Notes { get; set; }
        public List<PaperInvoiceModel> PaperInvoices { get; set; }
        public List<PaymentProfileModel> PaymentProfiles { get; set; }
        public List<PaymentTransactionModel> PaymentTransactions { get; set; }
        public List<RelationModel> RelationEntities { get; set; }
        public List<RelationModel> RelationRelatedEntities { get; set; }
        public List<GroupMemberModel> GroupMembers { get; set; }
        public List<EntityRoleModel> EntityRoles { get; set; }

        public bool IsMember
        {
            get
            {
                if (MembershipConnections != null)
                {
                    return MembershipConnections.Any(x => x.Membership.Status == (int)Status.Active);
                } 
                else
                {
                    return false;
                }
            }
        }
        public string MembershipType
        {
            get
            {
                if (MembershipConnections != null)
                {
                    if(MembershipConnections.Any(x => x.Membership.Status == (int)Status.Active))
                    {
                        return MembershipConnections.Where(x => x.Membership.Status == (int)Status.Active).Select(x => x.Membership.MembershipType == null ? "" : x.Membership.MembershipType.Name).FirstOrDefault();
                    }
                    else
                    {
                        return "Non Member";
                    }
                }
                else
                {
                    return "Non Member";
                }
            }
        }
        public string PreferredCommunication
        {
            get
            {
                if (PreferredBillingCommunication == (int)BillingCommunication.PaperInvoice)
                {
                    return "Paper Invoice";
                }
                else if (PreferredBillingCommunication == (int)BillingCommunication.EmailInvoice)
                {
                    return "Email";
                }
                else
                {
                    return "Paper Invoice & Email";
                }
            }
        }
    }
}
