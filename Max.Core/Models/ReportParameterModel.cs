using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReportParameterModel
    {
        public int ReportParameterId { get; set; }
        public int CategoryId { get; set; }
        public string Parameter { get; set; }
        public string Type { get; set; }
    }
}
