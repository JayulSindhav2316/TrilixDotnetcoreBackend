using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class PaymentRequestModel
    {
        public string OrganizationName { get; set; }
        public string PaymentToken { get; set; }
        public string IpAddress { get; set; }
    }
}
