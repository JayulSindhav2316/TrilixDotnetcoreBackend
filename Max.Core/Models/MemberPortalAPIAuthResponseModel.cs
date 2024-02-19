using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MemberPortalAPIAuthResponseModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public int OrganizationId { get; set; }
        public string RefreshToken { get; set; }
        public string IpAddress { get; set; }
        public string TenantId { get; set; }
        public string TenantCN { get; set; }
        public string TenantRCN { get; set; }
        public ResponseStatusModel ResponseStatus { get; set; }
        public string TenantName { get; set; }
    }
}
