using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.RefundModels
{
    public class VoidTransactionRequest
    {
        [JsonProperty(PropertyName = "createTransactionRequest")]
        public CreateTransactionRequest CreateTransactionRequest { get; set; }
    }
}
