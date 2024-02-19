using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.RefundModels
{
    public class CreditCard
    {
        [JsonProperty(PropertyName = "cardNumber")]
        public string CardNumber { get; set; }
        [JsonProperty(PropertyName = "expirationDate")]
        public string ExpirationDate { get; set; }
    }
}
