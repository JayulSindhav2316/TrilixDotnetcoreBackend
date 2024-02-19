using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class BankAccountPaymentProfile
    {

        public int PaymentProfileId { get; set; }
        public string AuthNetPaymentProfileId { get; set; }
        public string AccountType { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string NameOnAccount { get; set; }
        public string NickName { get; set; }
        public int PreferredPaymentMethod { get; set; }
        public int UseForAutoBilling { get; set; }
    }
}
