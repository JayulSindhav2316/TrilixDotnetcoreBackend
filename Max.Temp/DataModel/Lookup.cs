using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Lookup
    {
        public int LookupId { get; set; }
        public string Group { get; set; }
        public string Values { get; set; }
        public int? Status { get; set; }
    }
}
