using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class MemberAccountEmailModel
    {
        public string OrganizationName { get; set; }
        public string EmailAddress { get; set; }
        public string IpAddress { get; set; }
        public string Token { get; set; }
        public string SiteUrl { get; set; }
    }
}
