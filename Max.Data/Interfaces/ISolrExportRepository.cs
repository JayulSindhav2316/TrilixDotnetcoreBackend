using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;


namespace Max.Data.Interfaces
{
    public interface ISolrExportRepository : IRepository<Solrexport> 
    {
        Task<IEnumerable<Solrexport>> GetAllSolrDocumentsAsync();
    }
}
