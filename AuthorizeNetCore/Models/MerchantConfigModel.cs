using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.Models
{
    public class MerchantConfigModel
    {
        [JsonProperty(PropertyName = "acceptjsUrl")]
        public string AccepJSURL { get; set; }
        [JsonProperty(PropertyName = "apiLoginID")]
        public string LoginId { get; set; }
        [JsonProperty(PropertyName = "clientKey")]
        public string TransactionKey { get; set; }
    }
}
