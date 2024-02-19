using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AutoBillingConfiguration
    {
        public int AutoBillingProcessingDateId { get; set; }
        public int AutoBillingSettingId { get; set; }
        public bool AutoBillingEnabled { get; set; }
        public bool AutoBillingStatus { get; set; }
        public DateTime ThroughDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string NotificationEmails { get; set; }
        public string NotificationSMSNumbers { get; set; }
    }
}
