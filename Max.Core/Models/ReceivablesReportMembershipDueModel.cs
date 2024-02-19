using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReceivablesReportMembershipDueModel
    {
        public int EntityId{ get; set; }
        public int PersonId { get; set; }
        public string MemberName { get; set; }
        public string BillableMemberName { get; set; }
        public int InvoiceId { get; set; }
        public string CreatedDate { get; set; }
        public string DueDate { get; set; }
        public string Description { get; set; }
        public decimal TotalDue { get; set; }
        public decimal Paid { get; set; }
        public string BillingType { get; set; }
        public decimal Balance
        {
            get { return (TotalDue - Paid); }
        }

    }
}
