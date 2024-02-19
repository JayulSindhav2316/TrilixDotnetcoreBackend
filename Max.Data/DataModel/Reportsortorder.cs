using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Reportsortorder
    {
        public int ReportSortOrderId { get; set; }
        public string FieldName { get; set; }
        public string Order { get; set; }
        public string FieldPathName { get; set; }
        public int? ReportId { get; set; }

        public virtual Report Report { get; set; }
    }
}
