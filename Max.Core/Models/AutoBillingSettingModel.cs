using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AutoBillingSettingModel
    {
        public int AutoBillingsettingsId { get; set; }
        public int? EnableAutomatedBillingForMembership { get; set; }
        public string AutomatedCoordinatorForMembership { get; set; }
        public int? IsPauseOrEnableForMembership { get; set; }
        public string EmailForNotification { get; set; }
        public string SmsforNotification { get; set; }
    }
}
