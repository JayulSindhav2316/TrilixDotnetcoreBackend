using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EnumOptionListModel
    {
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}

