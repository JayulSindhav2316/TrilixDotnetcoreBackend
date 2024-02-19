using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class TokenRequestModel
    {
        public int UserId { get; set; }
        public int EntityId { get; set; }
        public string RefreshToken { get; set; }
        public string IpAddress { get; set; }
    }
}
