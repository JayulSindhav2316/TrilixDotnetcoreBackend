using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GlAccountTypeModel
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
    }
}
