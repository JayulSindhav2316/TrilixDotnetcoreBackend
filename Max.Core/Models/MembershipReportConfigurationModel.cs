using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipReportConfigurationModel
    {
        public int MembershipReportId { get; set; }
        public string Categories { get; set; }
        public string MembershipTypes { get; set; }
        public string Status { get; set; }
        public string RemovedUsers { get; set; }
        public string ReportType { get; set; }
        public int ReportCategoryId { get; set; }
        public int ReportId { get; set; }

    }
}
