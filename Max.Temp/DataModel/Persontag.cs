using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Persontag
    {
        public int TagId { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public int? PersonId { get; set; }

        public virtual Person Person { get; set; }
    }
}
