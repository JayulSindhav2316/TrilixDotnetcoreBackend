using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
    public class LineItems
    {
        [JsonProperty(PropertyName = "lineItem")]
        public LineItem[] LineItem { get; set; }
    }
}
