using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class LinkEventGroupModel
    {
        public LinkEventGroupModel()
        {
            GroupPriorityDateSettings = new List<LinkGroupRegistrationDatePriorityModel>();
            GroupPriorityFeeSettings = new List<SessionRegistrationGroupPricingModel>();
        }
        public int LinkEventGroupId { get; set; }
        public string GroupName { get; set; }
        public int? EventId { get; set; }
        public int? RegistrationGroupId { get; set; }
        public int? EnableOnlineRegistration { get; set; }
        public int? IsGroupLinked { get; set; }
        public List<LinkGroupRegistrationDatePriorityModel> GroupPriorityDateSettings { get; set; }
        public List<SessionRegistrationGroupPricingModel> GroupPriorityFeeSettings { get; set; }
    }
}
