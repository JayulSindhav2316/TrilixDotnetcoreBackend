using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Email
    {
        public int EmailId { get; set; }
        public string EmailAddressType { get; set; }
        public string EmailAddress { get; set; }
        public int? PersonId { get; set; }
        public int IsPrimary { get; set; }
        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
        public virtual Person Person { get; set; }
    }
}
