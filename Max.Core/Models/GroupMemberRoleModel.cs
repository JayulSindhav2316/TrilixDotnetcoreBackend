using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GroupMemberRoleModel
    {
        public int GroupId { get; set; }
        public int GroupMemberRoleId { get; set; }
        public int? GroupMemberId { get; set; }
        public int? GroupRoleId { get; set; }
        public string GroupRoleName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public bool IsActive
        {
            get
            {
                return Status == 1 ? true : false;
            }
            set
            {
                Status = value ? 1 : 0;
            }
        }
        public GroupRoleModel GroupRole { get; set; }
    }
}
