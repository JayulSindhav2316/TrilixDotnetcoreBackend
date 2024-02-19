using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
    public class PaymentTransactionResponce
    {
        [JsonProperty(PropertyName = "transactionResponse")]
        public TransactionResponse TransactionResponse { get; set; }
        [JsonProperty(PropertyName = "refId")]
        public string RefId { get; set; }
        [JsonProperty(PropertyName = "messages")]
        public Results Messages { get; set; }

    }
}
