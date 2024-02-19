using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EmailSettingModel
    {
        public string EmailServer { get; set; }
        public int PortNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
