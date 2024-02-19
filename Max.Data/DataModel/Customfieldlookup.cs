using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Customfieldlookup
    {
        public int Id { get; set; }
        public int? ModuleId { get; set; }
        public int? CustomFieldId { get; set; }
        public int? OrderOfDisplay { get; set; }
        public int? TabId { get; set; }
        public int? Status { get; set; }
        public int? BlockId { get; set; }

        public virtual Customfieldblock Block { get; set; }
        public virtual Customfield CustomField { get; set; }
        public virtual Moduleinfo Module { get; set; }
    }
}
