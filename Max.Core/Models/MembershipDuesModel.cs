using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipDuesModel
    {
        public int EntityId { get; set; }
        public string Name { get; set; }
        public int InvoiceId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        public string Frequency { get; set; }
        public decimal TotalDue { get; set; }
        public decimal Paid { get; set; }
        public string BillingType { get; set; }
        public string InvoiceType { get; set; }
        public decimal Balance
        {
            get { return (TotalDue - Paid); }
        }
        public dynamic ResponseObject { get; set; }
        public string Response { get; set; }

    }
}
