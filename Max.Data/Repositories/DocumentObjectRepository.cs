using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;

namespace Max.Data.Repositories
{
    public class DocumentObjectRepository : Repository<Documentobject>, IDocumentObjectRepository
    {
        public DocumentObjectRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Documentobject>> GetAllDocumentObjectsAsync()
        {
            return await membermaxContext.Documentobjects
                        .ToListAsync();
        }
        public async Task<IEnumerable<Documentobject>> GetAllDocumentObjectsByContainerIdAsync(int id)
        {
            return await membermaxContext.Documentobjects
                .Where(x => x.ContainerId ==id && x.Active==(int)Status.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Documentobject>> GetSubFoldersByContainerIdAsync(int id)
        {
            return await membermaxContext.Documentobjects
                .Where(x => x.ContainerId == id && x.FileType==0)
                .ToListAsync();
        }

        public async Task<Documentobject> GetDocumentObjectByIdAsync(int id)
        {
            return await membermaxContext.Documentobjects
                .Include(x => x.CreatedByNavigation)
                .Include(x => x.Documenttags)
                    .ThenInclude(x => x.Tag)
                .Where(x => x.DocumentObjectId == id)
                .FirstOrDefaultAsync();
        }
        public async Task<Documentobject> GetDocumentObjectByNameAsync(string name, string pathName)
        {
            return await membermaxContext.Documentobjects
                .Where(x => x.FileName == name && x.PathName==pathName)
                .FirstOrDefaultAsync();
        }
        
        public async Task<Documentobject> GetActiveDocumentObjectByNameAsync(string name, string pathName)
        {
            return await membermaxContext.Documentobjects
                .Where(x => x.FileName == name && x.PathName==pathName && x.Active==(int)Status.Active)
                .FirstOrDefaultAsync();
        }

        public async Task<Documentobject> GetDocumentObjectByContainerIdAndNameAsync(int containerId, string pathName, string fileName)
        {
            return await membermaxContext.Documentobjects
                .Where(x => x.FileName == fileName && x.ContainerId == containerId && x.PathName == pathName)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Documentobject>> GetChildDocumentObjectsByContainerIdAndPathNameAsync(int containerId, string pathName)
        {
            return await membermaxContext.Documentobjects
               .Where(x =>  x.ContainerId == containerId && x.PathName.StartsWith(pathName) && x.Active==(int)Status.Active)
               .ToListAsync();
        }
        public async Task<IEnumerable<Documentobject>> GetChildFolderssByContainerIdAndPathNameAsync(int containerId, string pathName)
        {
            return await membermaxContext.Documentobjects
               .Where(x => x.ContainerId == containerId && x.PathName == pathName && x.FileType == (int)DocumentObjectType.Folder)
               .ToListAsync();
        }
        public async Task<IEnumerable<Documentobject>> GetDocumentObjectsByContainerAndPathAsync(int containerId, string path)
        {
            return await membermaxContext.Documentobjects
                .Include(x => x.CreatedByNavigation)
                .Include(x => x.Documentaccesses)
                .Include(x => x.Documenttags)
                    .ThenInclude(x => x.Tag)
                .Where(x => x.PathName == path && x.ContainerId == containerId && x.FileType == (int)DocumentObjectType.File && x.Active == (int)Status.Active)
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
