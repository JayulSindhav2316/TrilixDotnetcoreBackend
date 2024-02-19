using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MemberLoginRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string OrganizationName { get; set; }
        public string IpAddress { get; set; }
        public string RequestFrom { get; set; }
        public double VerificationMinutes { get; set; }
    }
}
