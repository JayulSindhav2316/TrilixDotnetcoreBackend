using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipHistoryModel
    {
        public int MembershipHistoryId { get; set; }
        public int? MembershipId { get; set; }
        public DateTime? StatusDate { get; set; }
        public int? Status { get; set; }
        public int? ChangedBy { get; set; }
        public string Reason { get; set; }
        public int? PreviousMembershipId { get; set; }


    }
}
