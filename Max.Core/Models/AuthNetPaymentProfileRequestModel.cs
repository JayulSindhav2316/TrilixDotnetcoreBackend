using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AuthNetPaymentProfileRequestModel
    {
        public int PaymentProfileId { get; set; }
        public int EntityId { get; set; }
        public string ProfileId { get; set; }
        public string AuthNetPaymentProfileId { get; set; }
        public int OrganizationId { get; set; }
    }
}
