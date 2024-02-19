using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Token
    {
        public int TokenId { get; set; }
        public string ClientId { get; set; }
        public string TokenValue { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiryTime { get; set; }

        public virtual Staffuser User { get; set; }
    }
}
