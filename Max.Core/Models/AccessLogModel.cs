using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AccessLogModel
    {
        public int AccessLogId { get; set; }
        public string UserName { get; set; }
        public string UserAgent { get; set; }
        public string Referrer { get; set; }
        public string IpAddress { get; set; }
        public int Portal { get; set; }
        public DateTime AccessDate { get; set; }
    }
}
