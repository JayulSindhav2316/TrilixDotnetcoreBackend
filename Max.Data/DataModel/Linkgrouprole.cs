using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Linkgrouprole
    {
        public int LinkGroupRoleId { get; set; }
        public int? GroupRoleId { get; set; }
        public int? GroupId { get; set; }
        public int? IsLinked { get; set; }
        public int? OrganizationId { get; set; }
        public string GroupRoleName { get; set; }

        public virtual Group Group { get; set; }
        public virtual Grouprole GroupRole { get; set; }
    }
}
