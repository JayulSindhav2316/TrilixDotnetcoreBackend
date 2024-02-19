using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IDocumentObjectAccessHistoryRepository : IRepository<Documentobjectaccesshistory>
    {

        Task<IEnumerable<Documentobjectaccesshistory>> GetAllDocumentAccessHistoryAsync();
        Task<IEnumerable<Documentobjectaccesshistory>> GetDocumentAccessHistoryByDocumentObjectIdAsync(int id);
        Task<IEnumerable<Documentobjectaccesshistory>> GetDocumentAccessHistoryByEntityIdAsync(int id);
        Task<IEnumerable<GroupDataModel>> GetDocumentAccessHistoryByDocument();
    }
}
