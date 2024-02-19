using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class SolrSearchResults
    {
        public SolrSearchResults()
        {
            SolrDocuments = new List<SolrDocumentModel>();
        }

        public long TotalResults { get; set; }
        public List<SolrDocumentModel> SolrDocuments { get; set; }
    }
}
