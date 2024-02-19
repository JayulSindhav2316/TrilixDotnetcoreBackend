using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Core.Models
{
    public class PersonTagModel
    {
        public int TagId { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public int? PersonId { get; set; }

    }
}
