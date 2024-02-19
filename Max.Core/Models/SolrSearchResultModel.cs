using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class SolrSearchResultModel
    {
        
        public SolrSearchResultModel()
        {
            Documents = new List<DocumentObjectModel>();
        }

        public List<DocumentObjectModel> Documents { get; set; }
        public int StartPage { get; set; }
        public int TotalCount { get; set; }
        public long TotalMatchCount { get; set; }
        public string DisplayMessage { get; set; }
    }
}
