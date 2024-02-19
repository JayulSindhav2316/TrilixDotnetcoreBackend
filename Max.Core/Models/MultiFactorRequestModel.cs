using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MultiFactorRequestModel
    {
        public int UserId { get; set; }
        public int EntityId { get; set; }
        public string Mode { get; set; }
        public string VerificationToken { get; set; }
        public string VerificationCode { get; set; }
        public int RememberDevice { get; set; }
        public string TenantName { get; set; }
    }
}
