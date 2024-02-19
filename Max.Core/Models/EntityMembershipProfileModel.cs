using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EntityMembershipProfileModel
    {
        public EntityMembershipProfileModel()
        {
            MembershipHistory = new List<EntityMembershipHistoryModel>();
            MembershipBillingHistory = new List<InvoicePaymentModel>();
            BillingSchedule = new List<EntityBillingModel>();
        }
        public int EntityId { get; set; }
        public int ActiveMembershipId { get; set; }
        public string MembershipName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime NextBillDate { get; set; }
        public string Period { get; set; }
        public string BillingFrequency { get; set; }
        public decimal MembershipBalance { get; set; }
        public decimal AvailableCredit { get; set; }
        public string ProfileId { get; set; }
        public string PaymentProfileId { get; set; }
        public EntityModel BillableEntity { get; set; }       
        public List<EntityBillingModel> BillingSchedule { get; set; }
        public List<EntityMembershipHistoryModel> MembershipHistory { get; set; }
        public List<InvoicePaymentModel> MembershipBillingHistory { get; set; }
        public bool IsMember { get; set; }
        public int MaxUnits { get; set; }
    }
}
