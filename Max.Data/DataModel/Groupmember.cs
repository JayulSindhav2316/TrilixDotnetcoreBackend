using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Groupmember
    {
        public Groupmember()
        {
            Groupmemberroles = new HashSet<Groupmemberrole>();
        }

        public int GroupMemberId { get; set; }
        public int? EntityId { get; set; }
        public int? GroupId { get; set; }
        public int? Status { get; set; }
        public int? SocialGroupMemberId { get; set; }

        public virtual Entity Entity { get; set; }
        public virtual Group Group { get; set; }
        public virtual ICollection<Groupmemberrole> Groupmemberroles { get; set; }
    }
}
