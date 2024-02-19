using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class BillingAddressModel
    {
        public string BillToName{ get; set; }
        public string BillToEmail { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

    }
}
