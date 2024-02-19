using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Reportfilter
    {
        public int ReportFilterId { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public int? ReportFieldId { get; set; }
        public int? ReportId { get; set; }

        public virtual Reportfield Field { get; set; }
        public virtual Report Report { get; set; }
    }
}
