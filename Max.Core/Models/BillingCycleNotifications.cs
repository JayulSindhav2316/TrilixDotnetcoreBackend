using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class BillingCycleNotifications
    {
        public int BillingCycleNotificationId { get; set; }
        public int BillingCycleId { get; set; }
        public string CycleName { get; set; }
        public int status { get; set; }
        public string Notification { get; set; }
    }
}
