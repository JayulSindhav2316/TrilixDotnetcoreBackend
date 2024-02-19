using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MemberPortalVerificationResponseModel
    {
        public int EntityId { get; set; }
        public int PersonId { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
