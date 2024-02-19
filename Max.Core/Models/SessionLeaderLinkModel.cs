using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Max.Core.Models
{
    public class SessionLeaderLinkModel
    {
        public int SessionLeaderLinkId { get; set; }
        public int? SessionId { get; set; }
        public int? EntityId { get; set; }
        public string EntityName { get; set; }
        public string base64ProfileImageData { get; set; }
        public byte[] imageArray { get; set; }
    }
}
