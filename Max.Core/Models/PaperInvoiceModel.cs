using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class PaperInvoiceModel
    {
        public int PaperInvoiceId { get; set; }
        public int PaperBillingCycleId { get; set; }
        public int InvoiceId { get; set; }
        public int EntityId { get; set; }
        public string BillableName { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public InvoiceModel Invoice { get; set; }
        public string PreferredBillingNotifictaion { get; set; }
        public string MembershipCount { get; set; }
        public string MembershipType { get; set; }
        public DateTime MembershipEndDate { get; set; }
        public DateTime MembershipNewEndDate { get; set; }
        public int? AdditionalMembersCount { get; set; }
    }
}
