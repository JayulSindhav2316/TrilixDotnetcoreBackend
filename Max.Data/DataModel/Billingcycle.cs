using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Billingcycle
    {
        public Billingcycle()
        {
            Billingbatches = new HashSet<Billingbatch>();
            Billingjobs = new HashSet<Billingjob>();
            Paperinvoices = new HashSet<Paperinvoice>();
            BillingCycleNotifications = new HashSet<BillingCycleNotification>();
        }

        public int BillingCycleId { get; set; }
        public string CycleName { get; set; }
        public DateTime RunDate { get; set; }
        public DateTime ThroughDate { get; set; }
        public int Status { get; set; }
        public int CycleType { get; set; }

        public virtual ICollection<Billingbatch> Billingbatches { get; set; }
        public virtual ICollection<Billingjob> Billingjobs { get; set; }
        public virtual ICollection<Paperinvoice> Paperinvoices { get; set; }
        public virtual ICollection<BillingCycleNotification> BillingCycleNotifications { get; set; }
    }
}
