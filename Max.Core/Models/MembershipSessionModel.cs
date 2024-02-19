using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipSessionModel
    {
        public int MembershipId { get; set; }
        public int BillableEntityId { get; set; }
        public int EntityId { get; set; }
        public int PrimaryMemberEntityId { get; set; } // Added for LBOLT-1032
        public int MembershipTypeId { get; set; }
        public int[] MembershipFeeIds { get; set; }
        public int[] AdditionalPersons { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserId { get; set; }
        public string Notes { get; set; }
        public decimal[] MembershipFees { get; set; }
    }
}
