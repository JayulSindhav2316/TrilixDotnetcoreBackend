using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class SolrExportModel
    {
        public int SolrId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FileName { get; set; }
        public string Text { get; set; }
    }
}
