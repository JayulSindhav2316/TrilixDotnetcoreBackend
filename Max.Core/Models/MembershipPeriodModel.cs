using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipPeriodModel
    {
        public int MembershipPeriodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PeriodUnit { get; set; }
        public int Duration { get; set; }
        public int? Status { get; set; }
    }
}
