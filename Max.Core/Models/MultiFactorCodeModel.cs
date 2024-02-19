using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MultiFactorCodeModel
    {
        public int MultiFactorCodeId { get; set; }
        public int UserId { get; set; }
        public string CommunicationMode { get; set; }
        public DateTime CreatDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Token { get; set; }
        public string DeviceId { get; set; }
        public string IpAddress { get; set; }
        public int CodeExpired { get; set; }
        public int CodeUsed { get; set; }
        public string UserAgent { get; set; }
        public int Attempts { get; set; }
        public string AccessCode { get; set; }
        public string Otpcode { get; set; }

    }
}
