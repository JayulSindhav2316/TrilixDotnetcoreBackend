using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.RefundModels
{
    public class TransactionRequest
    {
        [JsonProperty(PropertyName = "transactionType")]
        public string TransactionType { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }
        [JsonProperty(PropertyName = "payment")]
        public Payment Payment { get; set; }
        [JsonProperty(PropertyName = "refTransId")]
        public string RefTransId { get; set; }
    }
}
