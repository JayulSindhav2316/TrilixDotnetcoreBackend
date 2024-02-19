using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Sessionleaderlink
    {
        public int SessionLeaderLinkId { get; set; }
        public int? SessionId { get; set; }
        public int? EntityId { get; set; }

        public virtual Entity Entity { get; set; }
        public virtual Session Session { get; set; }
    }
}
