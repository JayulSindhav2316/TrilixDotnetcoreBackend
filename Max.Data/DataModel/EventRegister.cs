using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Eventregister
    {
        public Eventregister()
        {
            Eventregisterquestions = new HashSet<Eventregisterquestion>();
            Eventregistersessions = new HashSet<Eventregistersession>();
        }

        public int EventRegisterId { get; set; }
        public int EventId { get; set; }
        public int EntityId { get; set; }

        public virtual Entity Entity { get; set; }
        public virtual Event Event { get; set; }
        public virtual ICollection<Eventregisterquestion> Eventregisterquestions { get; set; }
        public virtual ICollection<Eventregistersession> Eventregistersessions { get; set; }
    }
}
