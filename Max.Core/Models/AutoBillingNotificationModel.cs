using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AutoBillingNotificationModel
    {
        public int AutoBillingNotificationId { get; set; }
        public int? AbpdId { get; set; }
        public string BillingType { get; set; }
        public string InvoiceType { get; set; }
        public string NotificationType { get; set; }
        public DateTime? NotificationSentDate { get; set; }
    }
}
