using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
    public class AuthorizationIndicatorType
    {
        [JsonProperty(PropertyName = "authorizationIndicator")]
        public string AuthorizationIndicator { get; set; }
    }
}
