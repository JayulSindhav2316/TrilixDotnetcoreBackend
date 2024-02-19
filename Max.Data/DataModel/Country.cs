using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Country
    {
        public int CountryId { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int PhoneCode { get; set; }
        public string ZipFormat { get; set; }
        public string Regex { get; set; }
    }
}
