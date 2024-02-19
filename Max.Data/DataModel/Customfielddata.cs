using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Customfielddata
    {
        public int Id { get; set; }
        public int? CustomFieldId { get; set; }
        public string Value { get; set; }
        public int? EntityId { get; set; }

        public virtual Customfield CustomField { get; set; }
        public virtual Entity Entity { get; set; }
    }
}
