using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipModel
    {
        public MembershipModel()
        {
            Invoices = new List<InvoiceModel>();
            MembershipHistories = new List<MembershipHistoryModel>();
            BillingFees = new List<BillingFeeModel>();
            MembershipConnections = new List<MembershipConnectionModel>();
        }

        public int MembershipId { get; set; }
        public int? MembershipTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ReviewDate { get; set; }
        public int BillableEntityId { get; set; }
        public DateTime NextBillDate { get; set; }
        public int BillingOnHold { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime RenewalDate { get; set; }
        public DateTime TerminationDate { get; set; }
        public int Status { get; set; }
        public List<InvoiceModel> Invoices { get; set; }
        public List<BillingFeeModel> BillingFees { get; set; }
        public List<MembershipHistoryModel> MembershipHistories { get; set; } 
        public List<MembershipConnectionModel> MembershipConnections { get; set; } 
        public MembershipTypeModel MembershipType { get; set; }
    }
}
