using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Autobillingsetting
    {
        public int AutoBillingsettingsId { get; set; }
        public int? EnableAutomatedBillingForMembership { get; set; }
        public int? IsPauseOrEnableForMembership { get; set; }
        public string EmailForNotification { get; set; }
        public string SmsforNotification { get; set; }
    }
}
