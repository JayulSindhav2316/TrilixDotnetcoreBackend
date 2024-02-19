using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EventReportConfigurationModel
    {
       
        public int EventReportId { get; set; }
        public string Event { get; set; }
        public string Sessions { get; set; }
        public string RemovedUsers { get; set; }
        public string ReportType { get; set; }
        public int ReportCategoryId { get; set; }
        public int ReportId { get; set; }
    }
}
