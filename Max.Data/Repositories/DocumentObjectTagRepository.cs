using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class DocumentObjectTagRepository : Repository<Documenttag>, IDocumentObjectTagRepository
    {
        public DocumentObjectTagRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Documenttag>> GetAllDocumentTagsAsync()
        {
            return await membermaxContext.Documenttags
                .ToListAsync();
        }

        public async Task<Documenttag> GetDocumentTagByIdAsync(int id)
        {
            return await membermaxContext.Documenttags
                .SingleOrDefaultAsync(m => m.DocumentTagId == id);
        }

        public async Task<IEnumerable<Documenttag>> GetDocumentTagsByDocumentObjectIdAsync(int id)
        {
            return await membermaxContext.Documenttags
                .Include( x=> x.Tag)
                .Where(m => m.DocumentObjectId == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Documenttag>> GetDocumentTagsByTagIdAsync(int id)
        {
            return await membermaxContext.Documenttags
                .Where(m => m.TagId == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Documenttag>> GetDocumentObjectsByTagIdAsync(int tagId)
        {
            return await membermaxContext.Documenttags
              .Include(x => x.DocumentObject)
              .Where(m => m.TagId == tagId)
              .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
