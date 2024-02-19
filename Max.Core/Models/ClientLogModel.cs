using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ClientLogModel
    {
        public int ClientLogId { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string UserName { get; set; }
        public string OrganizationName { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
        public string Route { get; set; }
        public string ClientUrl { get; set; }
    }
}
