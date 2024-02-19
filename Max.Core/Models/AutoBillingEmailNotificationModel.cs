using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AutoBillingEmailNotificationModel
    {
        public int OrganizationId { get; set; }
        public string[] EmailAddresses { get; set; }
        public string Subject { get; set; }
        public string Title { get; set; }
        public string BillingType { get; set; }
        public string ProcessDate { get; set; }
        public string ThroughDate { get; set; }
        public decimal TotalDue { get; set; }
        public decimal Approved { get; set; }
        public decimal Declined { get; set; }
    }
}
