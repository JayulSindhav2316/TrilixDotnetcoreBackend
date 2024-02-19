using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
    public class ChargePaymentProfileTransactionRequest
    {
        [JsonProperty(PropertyName = "createTransactionRequest")]
        public ChargeProfileTransactionRequest CreateTransactionRequest { get; set; }
    }
}
