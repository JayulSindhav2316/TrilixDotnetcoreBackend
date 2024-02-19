using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class SessionRegistrationGroupPricingModel
    {
        public int SessionRegistrationGroupPricingId { get; set; }
        public int? SessionId { get; set; }
        public int? RegistrationGroupId { get; set; }
        public int? RegistrationFeeTypeId { get; set; }
        public decimal? Price { get; set; }
    }
}
