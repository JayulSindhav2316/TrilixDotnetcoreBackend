using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Customfieldoption
    {
        public int OptionId { get; set; }
        public int? CustomFieldId { get; set; }
        public string Option { get; set; }
        public string Code { get; set; }

        public virtual Customfield CustomField { get; set; }
    }
}
