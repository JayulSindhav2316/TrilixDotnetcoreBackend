using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;


namespace AuthorizeNetCore.VoidModels
{
    public class CreateTransactionRequest
    {
        [JsonProperty(PropertyName = "merchantAuthentication")]
        public MerchantAuthentication MerchantAuthentication { get; set; }
        [JsonProperty(PropertyName = "refId")]
        public string RefId { get; set; }
        [JsonProperty(PropertyName = "transactionRequest")]
        public TransactionRequest TransactionRequest { get; set; }
    }
}
