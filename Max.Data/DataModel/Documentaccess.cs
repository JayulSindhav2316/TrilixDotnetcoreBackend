using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Documentaccess
    {
        public int DocumentAccessId { get; set; }
        public int? DocumentObjectId { get; set; }
        public int? MembershipTypeId { get; set; }
        public int? GroupId { get; set; }
        public int? StaffRoleId { get; set; }

        public virtual Documentobject DocumentObject { get; set; }
        public virtual Group Group { get; set; }
        public virtual Membershiptype MembershipType { get; set; }
        public virtual Role StaffRole { get; set; }
    }
}
