using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipConnectionModel
    {
        public int MembershipConnectionId { get; set; }
        public int MembershipId { get; set; }
        //public int PersonId { get; set; }
        public int EntityId { get; set; }
        public int Status { get; set; }
        public  MembershipModel Membership { get; set; }
    }
}


