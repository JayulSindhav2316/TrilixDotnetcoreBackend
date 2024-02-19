using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class UserDeviceModel
    {
        public int UserDeviceId { get; set; }
        public int UserId { get; set; }
        public int EntityId { get; set; }
        public string Ipaddress { get; set; }
        public string DeviceName { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastAccessed { get; set; }
        public int Authenticated { get; set; }
        public int Locked { get; set; }
        public DateTime LastValidated { get; set; }
        public int RemberDevice { get; set; }

    }
}
