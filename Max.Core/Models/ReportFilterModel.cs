using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReportFilterModel
    {
        public int ReportFilterId { get; set; }
        public int ReportId { get; set; }
        public int ParameterId { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public int ReportFieldId { get; set; }
        
    }
}
