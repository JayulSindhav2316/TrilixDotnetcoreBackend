using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MenuItemModel
    {
        public MenuItemModel()
        {
            //MenuItems = new List<MenuItemModel>();
            RouterLink = new List<string>();
        }
        public bool? Display { get; set; }
        public string Label { get; set; }
        [JsonProperty(PropertyName = "expanded", NullValueHandling = NullValueHandling.Ignore)]
        public string Expanded { get; set; }
        [JsonProperty(PropertyName = "icon", NullValueHandling = NullValueHandling.Ignore)]
        public string Icon { get; set; }
        [JsonProperty(PropertyName = "routerLink", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> RouterLink { get; set; }
        [JsonProperty(PropertyName = "items", NullValueHandling = NullValueHandling.Ignore)]
        public List<MenuItemModel> MenuItems { get; set; }

    }
}
