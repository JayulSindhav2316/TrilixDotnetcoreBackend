using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class SelectListModel
    {
        public string code { get; set; }
        public string name { get; set; }
    }
    public class DropdownListModel : SelectListModel
    {
        public int value { get; set; }
    }
}
