using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class PaymentProfileModel
    {
        public PaymentProfileModel()
        {
            CreditCards = new List<CreditCardPaymentProfile>();
            BankAccounts = new List<BankAccountPaymentProfile>();
        }
        public string ProfileId { get; set; }
        public int EntityId { get; set; }
        public int Status { get; set; }
        public List<CreditCardPaymentProfile> CreditCards { get; set; }
        public List<BankAccountPaymentProfile> BankAccounts { get; set; }
    }
}
