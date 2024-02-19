using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Opportunityreport
    {

        public int OpportunityReportId { get; set; }
        public string Pipeline { get; set; }
        public string Products { get; set; }
        public string Stages { get; set; }
        public int? ReportId { get; set; }

        public virtual Report Report { get; set; }
    }
}
