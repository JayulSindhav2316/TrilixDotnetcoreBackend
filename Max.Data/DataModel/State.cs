using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class State
    {
        public int StateId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int CountryId { get; set; }
    }
}
