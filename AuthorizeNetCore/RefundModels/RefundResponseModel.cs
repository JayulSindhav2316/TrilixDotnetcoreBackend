using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.RefundModels
{
    public class VoidResponseModel
    {
        [JsonProperty(PropertyName = "transactionResponse")]
        public TransactionResponse TransactionResponse { get; set; }
        [JsonProperty(PropertyName = "refId")]
        public string RefId { get; set; }

        [JsonProperty(PropertyName = "messages")]
        public ResultMessage Messages { get; set; }
    }
}
