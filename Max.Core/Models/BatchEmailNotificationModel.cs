using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class BatchEmailNotificationModel
    {
        public int BillingEmailId { get; set; }
        public string EmailAddress { get; set; }
        public int InvoiceId { get; set; }
        public InvoiceModel Invoice { get; set; }
        public OrganizationModel Organization { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string PaymentUrl { get; set; }
        public string BaseUrl { get; set; }
    }
}
