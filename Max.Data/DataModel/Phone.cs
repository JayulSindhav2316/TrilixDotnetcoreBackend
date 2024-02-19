using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Phone
    {
        public int PhoneId { get; set; }
        public string PhoneType { get; set; }
        public string PhoneNumber { get; set; }
        public int? PersonId { get; set; }
        public int IsPrimary { get; set; }
        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
        public virtual Person Person { get; set; }
    }
}
