using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Services.Interfaces
{
    public interface ISolrIndexService<T>
    {
        bool AddUpdate(T document);
        bool Delete(T document);
        string ExtractDocumentText(string fileName);
        IEnumerable<SolrDocumentModel> GetDocumentsByText(string text, string filter, string tenantId);
        long GetSearchCountByText(string text, string filter, string tenantId);
        IEnumerable<SolrDocumentModel> ExportDocuments(int startPage);
    }
}
