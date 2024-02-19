using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReportFieldTableModel 
    {
        public ReportFieldTableModel()
        {
            Children = new List<ReportFieldTableModel>();
        }
        public string Label { get; set; }
        public string Table { get; set; }
        public int Data { get; set; }
        //public string DataType { get; set; }
        //public string DisplayType { get; set; }
        //public string ExpandedIcon { get; set; }
        //public string CollapsedIcon { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        public bool Selectable { get; set; }
        public List<ReportFieldTableModel> Children { get; set; }

    }
}
