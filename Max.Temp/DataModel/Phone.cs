using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Phone
    {
        public int PhoneId { get; set; }
        public string Label { get; set; }
        public string PhoneNumber { get; set; }
        public int? PersonId { get; set; }

        public virtual Person Person { get; set; }
    }
}
