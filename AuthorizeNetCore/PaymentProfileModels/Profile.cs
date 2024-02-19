using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
    public class Profile
    {
        [JsonProperty(PropertyName = "customerProfileId")]
        public string CustomerProfileId { get; set; }
        [JsonProperty(PropertyName = "paymentProfile")]
        public PaymentProfile PaymentProfile { get; set; }
    }
}
