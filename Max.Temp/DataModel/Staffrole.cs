using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Staffrole
    {
        public int StaffRoleId { get; set; }
        public int StaffId { get; set; }
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual Staffuser Staff { get; set; }
    }
}
