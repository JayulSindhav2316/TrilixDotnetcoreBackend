using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Paymentprofile
    {
        public int PaymentProfileId { get; set; }
        public int EntityId { get; set; }
        public string ProfileId { get; set; }
        public string AuthNetPaymentProfileId { get; set; }
        public string CardHolderName { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string AccountType { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string NameOnAccount { get; set; }
        public int UseForAutobilling { get; set; }
        public int? PreferredPaymentMethod { get; set; }
        public int Status { get; set; }
        public string NickName { get; set; }

        public virtual Entity Entity { get; set; }
    }
}
