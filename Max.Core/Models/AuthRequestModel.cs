using System;
using System.Collections.Generic;
using System.Text;
using Max.Core.Models;

namespace Max.Core.Models
{
    public class AuthRequestModel
    {
        public string OrganizationName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
        public int Portal { get; set; }
        public double VerificationMinutes { get; set; }

    }
}
