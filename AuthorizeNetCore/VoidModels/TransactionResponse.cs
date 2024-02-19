using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.VoidModels
{
    public class TransactionResponse
    {
        [JsonProperty(PropertyName = "responseCode")]
        public string ResponseCode { get; set; }
        [JsonProperty(PropertyName = "authCode")]
        public string AuthCode { get; set; }
        [JsonProperty(PropertyName = "avsResultCode")]
        public string AvsResultCode { get; set; }
        [JsonProperty(PropertyName = "cvvResultCode")]
        public string CvvResultCode { get; set; }
        [JsonProperty(PropertyName = "cavvResultCode")]
        public string CavvResultCode { get; set; }
        [JsonProperty(PropertyName = "transId")]
        public string TransId { get; set; }
        [JsonProperty(PropertyName = "refTransID")]
        public string RefTransID { get; set; }
        [JsonProperty(PropertyName = "transHash")]
        public string TransHash { get; set; }
        [JsonProperty(PropertyName = "accountNumber")]
        public string AccountNumber { get; set; }
        [JsonProperty(PropertyName = "accountType")]
        public string AccountType { get; set; }
        [JsonProperty(PropertyName = "messages")]
        public List<Message> Messages { get; set; }
    }
}
