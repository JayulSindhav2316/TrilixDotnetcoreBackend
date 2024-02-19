using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.RefundModels
{
    public class ResultMessage
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "resultCode")]
        public string ResultCode { get; set; }
        [JsonProperty(PropertyName = "message")]
        public List<Message> Message { get; set; }
    }
}

