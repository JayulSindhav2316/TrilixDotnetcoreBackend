using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IDocumentObjectRepository : IRepository<Documentobject>
    {

        Task<IEnumerable<Documentobject>> GetAllDocumentObjectsAsync();
        Task<IEnumerable<Documentobject>> GetAllDocumentObjectsByContainerIdAsync(int id);
        Task<IEnumerable<Documentobject>> GetSubFoldersByContainerIdAsync(int id);
        Task<Documentobject> GetDocumentObjectByIdAsync(int id);
        Task<Documentobject> GetDocumentObjectByNameAsync(string name, string path);
        Task<Documentobject> GetActiveDocumentObjectByNameAsync(string name, string path);
        Task<Documentobject> GetDocumentObjectByContainerIdAndNameAsync(int containerId, string pathName, string fileName);
        Task<IEnumerable<Documentobject>> GetDocumentObjectsByContainerAndPathAsync(int containerId, string path);
        Task<IEnumerable<Documentobject>> GetChildDocumentObjectsByContainerIdAndPathNameAsync(int containerId, string pathName);
        Task<IEnumerable<Documentobject>> GetChildFolderssByContainerIdAndPathNameAsync(int containerId, string pathName);

    }
}
