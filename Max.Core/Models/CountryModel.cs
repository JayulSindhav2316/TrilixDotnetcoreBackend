using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class CountryModel
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string PhoneCode { get; set; }
        public string ZipFormat { get; set; }
        public string Regex { get; set; }
    }
}
