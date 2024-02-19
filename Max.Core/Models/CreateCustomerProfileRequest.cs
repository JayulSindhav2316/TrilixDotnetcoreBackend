using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Authnet.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class MerchantAuthentication
    {
        public string name { get; set; }
        public string transactionKey { get; set; }
    }

    public class OpaqueData
    {
        [JsonProperty(PropertyName = "dataDescriptor")]
        public string dataDescriptor { get { return "COMMON.ACCEPT.INAPP.PAYMENT"; } }
        [JsonProperty(PropertyName = "dataValue")]
        public string nonceValue { get; set; }
    }
    public class Payment
    {
        public OpaqueData opaqueData { get; set; }
    }

    public class PaymentProfiles
    {
        public string customerType { get; set; }
        public Payment payment { get; set; }
    }

    public class Profile
    {
        public string merchantCustomerId { get; set; }
        public string description { get; set; }
        public string email { get; set; }
        public PaymentProfiles paymentProfiles { get; set; }
    }

    public class CreateCustomerProfileRequest
    {
        public MerchantAuthentication merchantAuthentication { get; set; }
        public Profile profile { get; set; }
        public string validationMode { get; set; }
    }

    public class RequestRoot
    {
        public CreateCustomerProfileRequest createCustomerProfileRequest { get; set; }
    }


}
