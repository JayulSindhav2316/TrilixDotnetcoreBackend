using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ResetPasswordRequestModel
    {
        public string Email { get; set; }
        public string OrganizationName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string IpAddress { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public int Portal { get; set; }
    }
}
