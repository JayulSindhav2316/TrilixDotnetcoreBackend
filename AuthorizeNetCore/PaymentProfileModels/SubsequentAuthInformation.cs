using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
    public class SubsequentAuthInformation
    {
        [JsonProperty(PropertyName = "originalNetworkTransId")]
        public string OriginalNetworkTransId { get; set; }
        [JsonProperty(PropertyName = "originalAuthAmount")]
        public string OriginalAuthAmount { get; set; }
        [JsonProperty(PropertyName = "Reason")]
        public string reason { get; set; }
    }
}
