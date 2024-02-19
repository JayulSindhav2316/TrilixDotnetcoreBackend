using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IDocumentObjectAccessRepository : IRepository<Documentaccess>
    {

        Task<IEnumerable<Documentaccess>> GetAllDocumentAccessAsync();
        Task<IEnumerable<Documentaccess>> GetDocumentAccessByDocumentObjectIdAsync(int id);
        Task<Documentaccess> GetDocumentObjectAccessByIdAsync(int id);
        Task<IEnumerable<Documentaccess>> GetDocumentObjectAccessListByMembershipTypeIdAsync(int id);
        Task<IEnumerable<Documentaccess>> GetDocumentObjectAccessListByGroupIdAsync(int id);
        Task<IEnumerable<Documentaccess>> GetDocumentObjectAccessListByStaffRoleIdAsync(int id);
    }
}
