using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GroupModel
    {
        public GroupModel()
        {
            Roles = new List<LinkGroupRoleModel>();
            GroupMembers = new List<GroupMemberModel>();
        }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public int? PreferredNumbers { get; set; }
        public int? ApplyTerm { get; set; }
        public DateTime? TerrmStartDate { get; set; }
        public DateTime? TermEndDate { get; set; }
        public int? OrganizationId { get; set; }
        public int? Status { get; set; }
        public int? Sync { get; set; }
        public int? GroupMembersCount { get; set; }

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

        public bool IsTermApplied
        {
            get
            {
                return ApplyTerm == 1 ? true : false;
            }
            set
            {
                ApplyTerm = value ? 1 : 0;
            }
        }

        public bool IsSynced
        {
            get
            {
                return Sync == 1 ? true : false;
            }
            set
            {
                Sync = value ? 1 : 0;
            }
        }

        public List<LinkGroupRoleModel> Roles { get; set; }
        public List<GroupMemberModel> GroupMembers { get; set; }
    }
}
