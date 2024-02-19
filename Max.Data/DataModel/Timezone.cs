using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Timezone
    {
        public Timezone()
        {
            Events = new HashSet<Event>();
        }

        public int TimeZoneId { get; set; }
        public string TimeZoneAbbreviation { get; set; }
        public string TimeZoneName { get; set; }
        public decimal? TimeZoneOffset { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
