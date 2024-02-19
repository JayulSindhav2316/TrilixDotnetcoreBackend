using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Containeraccess
    {
        public uint ContainerAccessId { get; set; }
        public int? ContainerId { get; set; }
        public int? MembershipTypeId { get; set; }
        public int? GroupId { get; set; }
        public int? StaffRoleId { get; set; }

        public virtual Documentcontainer Container { get; set; }
        public virtual Group Group { get; set; }
        public virtual Membershiptype MembershipType { get; set; }
        public virtual Role StaffRole { get; set; }
    }
}
