using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipChangeModel
    {
        public int PersonId { get; set; }
        public int MembershipId { get; set; }
        public string ChangeStatus { get; set; }
        public string Reason { get; set; }
    }
}
