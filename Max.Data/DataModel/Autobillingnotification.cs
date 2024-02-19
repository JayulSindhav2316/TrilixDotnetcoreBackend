using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Autobillingnotification
    {
        public int AutoBillingNotificationId { get; set; }
        public int? AbpdId { get; set; }
        public string BillingType { get; set; }
        public string InvoiceType { get; set; }
        public string NotificationType { get; set; }
        public DateTime? NotificationSentDate { get; set; }
    }
}
