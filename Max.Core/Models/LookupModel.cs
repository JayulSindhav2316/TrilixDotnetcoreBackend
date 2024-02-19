using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class LookupModel
    {
        public int LookupId { get; set; }
        public string Group { get; set; }
        public string Values { get; set; }
        public int? Status { get; set; }
    }
}
