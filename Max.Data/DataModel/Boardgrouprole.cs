using System;
using System.Collections.Generic;

namespace Max.Data.DataModel
{
    public partial class Boardgrouprole
    {
        public int BoardGroupRoleId { get; set; }
        public string BoardGroupRoleName { get; set; }
        public int? OrganizationId { get; set; }
    }
}
