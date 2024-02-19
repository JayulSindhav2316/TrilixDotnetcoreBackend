using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
    public class Results
    {
        [JsonProperty(PropertyName = "resultCode")]
        public string ResultCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public ResultMessage[] ResultMessages { get; set; }
    }
}
