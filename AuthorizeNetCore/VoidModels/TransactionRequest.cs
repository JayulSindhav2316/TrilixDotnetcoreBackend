using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.VoidModels
{
    public class TransactionRequest
    {
        [JsonProperty(PropertyName = "transactionType")]
        public string TransactionType { get; set; }
       
        [JsonProperty(PropertyName = "refTransId")]
        public string RefTransId { get; set; }
    }
}
