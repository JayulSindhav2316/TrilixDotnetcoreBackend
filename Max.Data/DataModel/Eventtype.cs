using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Eventtype
    {
        public Eventtype()
        {
            Events = new HashSet<Event>();
        }

        public int EventTypeId { get; set; }
        public string EventType1 { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
