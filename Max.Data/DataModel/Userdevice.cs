using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Userdevice
    {
        public int UserDeviceId { get; set; }
        public int UserId { get; set; }
        public int? EntityId { get; set; }
        public string Ipaddress { get; set; }
        public string DeviceName { get; set; }
        public DateTime? Created { get; set; }
        public DateTime LastAccessed { get; set; }
        public DateTime? LastValidated { get; set; }
        public int Authenticated { get; set; }
        public int Locked { get; set; }
        public int? RemberDevice { get; set; }

        public virtual Entity Entity { get; set; }
        public virtual Staffuser User { get; set; }
    }
}
