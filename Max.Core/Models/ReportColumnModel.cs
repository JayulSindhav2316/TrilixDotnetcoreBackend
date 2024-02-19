using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReportColumnModel
    {
        public ReportColumnModel(string field, string header)
        {
            this.Field = field;
            this.Header = header;
        }
        public string Field { get; set; }
        public string Header { get; set; }
        public string Type { get; set; }
    }
}
