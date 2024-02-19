using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Max.Core.Models
{
    public class LinkGroupRoleModel
    {
        public int LinkGroupRoleId { get; set; }
        public int? GroupRoleId { get; set; }
        public int? GroupId { get; set; }
        public int? IsLinked { get; set; }
        public int? OrganizationId { get; set; }
        public string GroupRoleName { get; set; }
        public int IsDefault { get; set; }


    }
}
