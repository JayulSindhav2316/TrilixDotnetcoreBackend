using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Email
    {
        public int EmailId { get; set; }
        public string Label { get; set; }
        public string EmailAddress { get; set; }
        public int? PersonId { get; set; }

        public virtual Person Person { get; set; }
    }
}
