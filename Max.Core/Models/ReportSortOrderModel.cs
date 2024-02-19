using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReportSortOrderModel
    {
        public int ReportSortOrderId { get; set; }
        public int ReportId { get; set; }
        public string FieldName { get; set; }
        public string Order { get; set; }

    }
}
