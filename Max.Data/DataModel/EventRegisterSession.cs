using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Eventregistersession
    {
        public int EventRegisterSessionId { get; set; }
        public int EventRegisterId { get; set; }
        public int SessionId { get; set; }
        public decimal? Price { get; set; }

        public virtual Eventregister EventRegister { get; set; }
        public virtual Session Session { get; set; }
    }
}
