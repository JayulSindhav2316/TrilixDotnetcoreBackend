using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.DataModel
{
    public class BillingCycleNotification
    {
        public int NotificationId { get; set; }
        public string NotificationText { get; set; }
        public int BillingCycleId { get; set; }
        public int IsRead { get; set; }

        public virtual Billingcycle BillingCycle { get; set; }
    }
}
