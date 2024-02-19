using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Max.Core.Helpers.Pdf;

namespace Max.Core.Models
{
    public class ReportModel
    {
        public ReportModel()
        {
            //ReportArguments = new List<ReportArgumentModel>();
            Columns = new List<ReportColumnModel>();
        }

        public int ReportId { get; set; }
        public int OrganizationId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public int? UserId { get; set; }
        public sbyte Shared { get; set; }

        public List<ReportColumnModel> Columns { get; set; }
        public DataTable Rows { get; set; }
        //public List<ReportArgumentModel> ReportArguments { get; set; }
    }
}
