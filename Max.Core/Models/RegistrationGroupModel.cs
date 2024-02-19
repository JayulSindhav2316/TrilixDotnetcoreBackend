using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class RegistrationGroupModel
    {
        public int RegistrationGroupId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? Status { get; set; }
        public List<int> MembershipTypeIds { get; set; }
    }
}
