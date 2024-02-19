using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IDocumentContainerRepository : IRepository<Documentcontainer>
    {

        Task<IEnumerable<Documentcontainer>> GetAllDocumentContainersAsync();
        Task<IEnumerable<Documentcontainer>> GetAllDocumentContainersWithObjectsAsync(int? entityId);
        Task<Documentcontainer> GetDocumentContainerByIdAsync(int id);
        Task<Documentcontainer> GetDocumentContainerByNameAsync(string name,int id);

    }
}
