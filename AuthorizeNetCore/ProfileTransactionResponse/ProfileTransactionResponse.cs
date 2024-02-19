using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.ProfileTransactionResponse
{
    public class ProfileTransactionResponse
    {
        public TransactionResponse transactionResponse { get; set; }
        public string refId { get; set; }
        public ResponseMessage messages { get; set; }
    }

    public class ResponseMessage
    {
        public string resultCode { get; set; }
        public List<Message> message { get; set; }
    }
    public class TransactionResponseMessage
    {
        public string code { get; set; }
        public string description { get; set; }
       
    }

    public class Profile
    {
        public string customerProfileId { get; set; }
        public string customerPaymentProfileId { get; set; }
    }

    public class TransactionResponse
    {
        public string responseCode { get; set; }
        public string authCode { get; set; }
        public string avsResultCode { get; set; }
        public string cvvResultCode { get; set; }
        public string cavvResultCode { get; set; }
        public string transId { get; set; }
        public string refTransID { get; set; }
        public string transHash { get; set; }
        public string testRequest { get; set; }
        public string accountNumber { get; set; }
        public string accountType { get; set; }
        public List<Message> messages { get; set; }
        public string transHashSha2 { get; set; }
        public Profile profile { get; set; }
        public int SupplementalDataQualificationIndicator { get; set; }
        public string networkTransId { get; set; }
    }

    public class Message
    {
        public string Code { get; set; }
        public string Text { get; set; }
    }

    public class ResultMessage
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }

}
