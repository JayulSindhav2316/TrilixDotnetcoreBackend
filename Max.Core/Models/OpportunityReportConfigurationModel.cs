using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class OpportunityReportConfigurationModel
    {
        public int OpportunityReportId { get; set; }
        public string Pipeline { get; set; }
        public string Products { get; set; }
        public string Stages { get; set; }
        public string RemovedUsers { get; set; }
        public string ReportType { get; set; }
        public int ReportCategoryId { get; set; }
        public int ReportId { get; set; }

    }
}
