using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Reportfield
    {
        public int ReportFieldId { get; set; }
        public int? ReportCategoryId { get; set; }
        public string FieldName { get; set; }
        public string TableName { get; set; }
        public string Label { get; set; }
        public string FieldTitle { get; set; }
        public string DataType { get; set; }
        public int DisplayOrder { get; set; }
        public string DisplayType { get; set; }
        public int? CustomFieldId { get; set; }

        public virtual Reportcategory ReportCategory { get; set; }
        public virtual Customfield CustomField { get; set; }
        public virtual ICollection<Reportfilter> Reportfilters { get; set; }
    }
}
