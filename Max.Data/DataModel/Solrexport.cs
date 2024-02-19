using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Solrexport
    {
        public int SolrId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FileName { get; set; }
        public string Text { get; set; }
        public int? Uploaded { get; set; }
    }
}
