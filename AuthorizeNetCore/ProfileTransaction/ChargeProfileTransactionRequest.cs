using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.ProfileTransaction
{
    public class MerchantAuthentication
    {
        public string name { get; set; }
        public string transactionKey { get; set; }
    }

    public class PaymentProfile
    {
        public string paymentProfileId { get; set; }
    }

    public class Profile
    {
        public string customerProfileId { get; set; }
        public PaymentProfile paymentProfile { get; set; }
    }

    public class LineItem
    {
        [JsonProperty(PropertyName = "itemId")]
        public string ItemId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "quantity")]
        public string Quantity { get; set; }
        [JsonProperty(PropertyName = "unitPrice")]
        public string UnitPrice { get; set; }

        public LineItem()
        {
            UnitPrice = "0";
            Quantity = "0";
        }
    }

    public class LineItems
    {
        [JsonProperty(PropertyName = "lineItem")]
        public LineItem[] lineItem { get; set; }
    }

    public class ProcessingOptions
    {
        public string isSubsequentAuth { get; set; }
    }

    public class SubsequentAuthInformation
    {
        public string originalNetworkTransId { get; set; }
        public string originalAuthAmount { get; set; }
        public string reason { get; set; }
    }

    public class AuthorizationIndicatorType
    {
        public string authorizationIndicator { get; set; }
    }

    public class TransactionRequest
    {
        public string transactionType { get; set; }
        public string amount { get; set; }
        public Profile profile { get; set; }
        public LineItems lineItems { get; set; }
        public ProcessingOptions processingOptions { get; set; }
        public SubsequentAuthInformation subsequentAuthInformation { get; set; }
        public AuthorizationIndicatorType authorizationIndicatorType { get; set; }
    }

    public class ChargeProfileTransactionRequest
    {
        public MerchantAuthentication merchantAuthentication { get; set; }
        public string refId { get; set; }
        public TransactionRequest transactionRequest { get; set; }
    }

    public class ChargePaymentProfileTransactionRequest
    {
        public ChargeProfileTransactionRequest createTransactionRequest { get; set; }
    }


}
