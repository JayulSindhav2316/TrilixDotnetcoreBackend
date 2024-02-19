using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
    public class ProfileTransactionResponse
    {
        [JsonProperty(PropertyName = "TransactionResponse")]
        public TransactionResponse TransactionResponse { get; set; }
        [JsonProperty(PropertyName = "RefId")]
        public string refId { get; set; }
        [JsonProperty(PropertyName = "Messages")]
        public ResponseMessage Messages { get; set; }
    }
}
