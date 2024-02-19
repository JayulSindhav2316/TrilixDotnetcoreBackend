using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipReportModel
    {
        public MembershipReportModel()
        {
            Columns = new List<ReportColumnModel>();
        }
        public string Title { get; set; }
        public List<ReportColumnModel> Columns { get; set; }
        public DataTable Rows { get; set; }
    }
}
