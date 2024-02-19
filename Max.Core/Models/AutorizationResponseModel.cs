using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AutorizationResponseModel
    {
        public int UserId { get; set; }
        public string emailDisplay { get; set; }
        public string PhoneDisplay { get; set; }
        public bool VerificationRequired { get; set; }
        public string VerificationToken { get; set; }
        public  ResponseStatusModel ResponseStatus { get; set; }
        public string TenantName { get; set; }
        public double VerificationTimeLimit { get; set; }
    }
}
