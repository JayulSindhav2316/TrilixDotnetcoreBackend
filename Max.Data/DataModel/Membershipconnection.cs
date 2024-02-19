using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Membershipconnection
    {
        public int MembershipConnectionId { get; set; }
        public int MembershipId { get; set; }
        public int EntityId { get; set; }
        public int Status { get; set; }

        public virtual Entity Entity { get; set; }
        public virtual Membership Membership { get; set; }
    }
}
