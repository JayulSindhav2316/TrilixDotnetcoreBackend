using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IDocumentObjectTagRepository : IRepository<Documenttag>
    {

        Task<IEnumerable<Documenttag>> GetAllDocumentTagsAsync();
        Task<Documenttag> GetDocumentTagByIdAsync(int id);
        Task<IEnumerable<Documenttag>> GetDocumentTagsByDocumentObjectIdAsync(int id);
        Task<IEnumerable<Documenttag>> GetDocumentTagsByTagIdAsync(int id);
        Task<IEnumerable<Documenttag>> GetDocumentObjectsByTagIdAsync(int tagId);
    }
}
