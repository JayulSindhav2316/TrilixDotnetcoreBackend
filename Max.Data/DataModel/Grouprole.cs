using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Grouprole
    {
        public Grouprole()
        {
            Groupmemberroles = new HashSet<Groupmemberrole>();
            Linkgrouproles = new HashSet<Linkgrouprole>();
        }

        public int GroupRoleId { get; set; }
        public string GroupRoleName { get; set; }
        public int? OrganizationId { get; set; }

        public virtual ICollection<Groupmemberrole> Groupmemberroles { get; set; }
        public virtual ICollection<Linkgrouprole> Linkgrouproles { get; set; }
    }
}
