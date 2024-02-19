using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Multifactorcode
    {
        public int MultiFactorCodeId { get; set; }
        public int UserId { get; set; }
        public int? EntityId { get; set; }
        public string CommunicationMode { get; set; }
        public DateTime CreatDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Token { get; set; }
        public int? DeviceId { get; set; }
        public string IpAddress { get; set; }
        public int CodeExpired { get; set; }
        public int CodeUsed { get; set; }
        public string UserAgent { get; set; }
        public int Attempts { get; set; }
        public string AccessCode { get; set; }
        public string Otpcode { get; set; }

        public virtual Entity Entity { get; set; }
        public virtual Staffuser User { get; set; }
    }
}
