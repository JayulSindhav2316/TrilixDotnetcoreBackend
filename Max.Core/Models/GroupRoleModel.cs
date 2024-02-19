using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public partial class GroupRoleModel
    {
        public int GroupRoleId { get; set; }
        public string GroupRoleName { get; set; }
        public int? OrganizationId { get; set; }
    }
}
