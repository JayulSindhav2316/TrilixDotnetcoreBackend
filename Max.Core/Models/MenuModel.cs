using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MenuModel
    {
        public MenuModel()
        {
            MenuItems = new List<MenuItemModel>();
        }
        public string Label { get; set; }
        public string Icon { get; set; }

        [JsonProperty(PropertyName = "items")]
        public List<MenuItemModel> MenuItems { get; set; }
    }
}
