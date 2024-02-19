using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReportFieldModel
    {
        public int ReportFieldId { get; set; }
        public int ReportCategoryId { get; set; }
        public string FieldName { get; set; }
        public string TableName { get; set; }
        public string FieldTitle { get; set; }
        public string DataType { get; set; }
        public int Sortable { get; set; }
    }
}
