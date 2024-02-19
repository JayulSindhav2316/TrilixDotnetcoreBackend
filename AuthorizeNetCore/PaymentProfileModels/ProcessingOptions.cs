using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
    public class ProcessingOptions
    {
        [JsonProperty(PropertyName = "IsSubsequentAuth")]
        public string isSubsequentAuth { get; set; }
    }
}
