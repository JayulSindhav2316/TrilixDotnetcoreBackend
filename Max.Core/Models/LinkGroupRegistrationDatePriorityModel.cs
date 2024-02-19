using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class LinkGroupRegistrationDatePriorityModel
    {
        public int LinkRegistrationGroupFeeId { get; set; }
        public int? RegistrationFeeTypeId { get; set; }
        public int? RegistrationGroupId { get; set; }
        public int? LinkEventGroupId { get; set; }
        public DateTime? RegistrationGroupDateTime { get; set; }
        public DateTime? RegistrationGroupEndDateTime { get; set; }
    }
}
