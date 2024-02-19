using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
    public class TransactionMessage
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}
