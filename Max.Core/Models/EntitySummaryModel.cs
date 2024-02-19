using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EntitySummaryModel
    {

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
        public int? PreferredBillingCommunication { get; set; }
        public bool? IsMember { get; set; }
        public bool? IsBillableNonMember { get; set; }
        public CompanyModel Company { get; set; }
        public OrganizationModel Organization { get; set; }
        public PersonModel Person { get; set; }
        public List<NotesModel> Notes { get; set; }
        public int IsBillable { get; set; }

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
                else if (PreferredBillingCommunication == (int)BillingCommunication.PaperAndEmail)
                {
                    return "Paper Invoice & Email";
                }
                else
                {
                    return "";
                }    
            }
        }
    }
}
