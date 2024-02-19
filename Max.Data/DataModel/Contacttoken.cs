using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Contacttoken
    {
        public int ContactTokenId { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public DateTime? Create { get; set; }
        public DateTime? Expire { get; set; }
        public int? Status { get; set; }
        public string IpAddress { get; set; }
    }
}
