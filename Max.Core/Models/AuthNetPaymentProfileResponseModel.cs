using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AuthNetPaymentProfileResponseModel
    {
        public int EntityId { get; set; }
        public string ProfileId { get; set; }
        public string AuthNetPaymentProfileId { get; set; }
        public string ErrorMessage { get; set; }
        public string AccountNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string CardType { get; set; }
    }
}
