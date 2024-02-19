using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class CreditCardPaymentProfile
    {
        public int PaymentProfileId { get; set; }
        public string AuthNetPaymentProfileId { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string CardType { get; set; }
        public string CardHolderName { get; set; }
        public int PreferredPaymentMethod { get; set; }
        public int UseForAutoBilling { get; set; }
    }
}
