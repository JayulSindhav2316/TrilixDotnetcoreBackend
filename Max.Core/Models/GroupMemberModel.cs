using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GroupMemberModel
    {
        public GroupMemberModel()
        {
            GroupMemberRoles = new List<GroupMemberRoleModel>();
        }
        public int GroupMemberId { get; set; }
        public int? EntityId { get; set; }
        public int? GroupId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public string CellPhoneNumber { get; set; }
        public string Email { get; set; }
        public string EntityName { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public int GroupStatus { get; set; }
        public GroupModel Group { get; set; }
        public GroupRoleModel Role { get; set; }
        public EntityModel Entity { get; set; }
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
        public string FormattedPhoneNumber
        {
            get
            {
                return CellPhoneNumber.FormatPhoneNumber();
            }
        }

        public bool IsGroupActive
        {
            get
            {
                return Group.Status == 1;
            }
        }
        public List<GroupMemberRoleModel> GroupMemberRoles { get; set; }
    }
}
