﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
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
}
