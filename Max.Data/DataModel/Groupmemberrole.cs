using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Groupmemberrole
    {
        public int GroupMemberRoleId { get; set; }
        public int? GroupMemberId { get; set; }
        public int? GroupRoleId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }

        public virtual Groupmember GroupMember { get; set; }
        public virtual Grouprole GroupRole { get; set; }
    }
}
