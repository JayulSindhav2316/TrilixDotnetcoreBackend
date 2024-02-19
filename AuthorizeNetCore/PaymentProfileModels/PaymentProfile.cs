using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
    public class PaymentProfile
    {
        [JsonProperty(PropertyName = "paymentProfileId")]
        public string PaymentProfileId { get; set; }
    }
}
