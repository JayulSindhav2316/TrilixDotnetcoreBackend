using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EventRegistrationSessionGroupAndPricingModel
    {
        public int SessionRegistrationGroupPricingId { get; set; }
        public int SessionId { get; set; }
        public string GroupName { get; set; }
        public string GroupPricing { get; set; }
        public int? GroupId { get; set; }
        public decimal? Pricing { get; set; }
        public int? SelectedItem { get; set; }
    }
}
