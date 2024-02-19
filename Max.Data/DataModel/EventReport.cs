using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Eventreport
    {

        public int EventReportId { get; set; }
        public string Event { get; set; }
        public string Sessions { get; set; }
        public int? ReportId { get; set; }

        public virtual Report Report { get; set; }
    }
}
