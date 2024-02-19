using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GrouphistoryModel
    {
        public int GroupHistoryId { get; set; }
        public int? GroupId { get; set; }
        public int? GroupMemberId { get; set; }
        public int? GroupRoleId { get; set; }
        public int? LinkGroupRoleId { get; set; }
        public string ActivityType { get; set; }
        public DateTime? ActivityDate { get; set; }
        public int? ActivityStaffId { get; set; }
        public string ActivityDescription { get; set; }
    }
}
