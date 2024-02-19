using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipCancelModel
    {
        public int PersonId { get; set; }
        public int UserId { get; set; }
        public int MembershipId { get; set; }
        public int InvoiceId { get; set; }
    }
}
