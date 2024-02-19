using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
    public class TransactionRequest
    {
        [JsonProperty(PropertyName = "transactionType")]
        public string TransactionType { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }
        [JsonProperty(PropertyName = "profile")]
        public Profile Profile { get; set; }

        [JsonProperty(PropertyName = "order")]
        public Order Order { get; set; }

        [JsonProperty(PropertyName = "lineItems")]
        public LineItems LineItems { get; set; }
        [JsonProperty(PropertyName = "processingOptions")]
        public ProcessingOptions ProcessingOptions { get; set; }
        [JsonProperty(PropertyName = "subsequentAuthInformation")]
        public SubsequentAuthInformation SubsequentAuthInformation { get; set; }
        [JsonProperty(PropertyName = "authorizationIndicatorType")]
        public AuthorizationIndicatorType AuthorizationIndicatorType { get; set; }
    }
}
