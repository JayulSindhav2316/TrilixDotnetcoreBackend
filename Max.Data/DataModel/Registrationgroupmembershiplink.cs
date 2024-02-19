using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Registrationgroupmembershiplink
    {
        public int RegistrationGroupMembershipLinkId { get; set; }
        public int? RegistrationGroupId { get; set; }
        public int? MembershipId { get; set; }

        public virtual Membershiptype Membership { get; set; }
        public virtual Registrationgroup RegistrationGroup { get; set; }
    }
}
