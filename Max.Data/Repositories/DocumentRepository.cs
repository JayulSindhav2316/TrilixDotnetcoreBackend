using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class DocumentRepository : Repository<Document>, IDocumentRepository
    {
        public DocumentRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
        {
            return await membermaxContext.Documents
                .ToListAsync();
        }
        public async Task<IEnumerable<Document>> GetDocumentsByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Documents
                .Where(x => x.EntityId == entityId)
                .ToListAsync();
        }

        public async Task<Document> GetDocumentByIdAsync(int id)
        {
            return await membermaxContext.Documents
                .SingleOrDefaultAsync(m => m.DocumentId == id);
        }

        public async Task<Document> GetPictureByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Documents
                 .Where(x => x.EntityId == entityId)
                .FirstOrDefaultAsync();
        }

        public async Task<Document> GetProfilePictureByStaffIdAsync(int staffId)
        {
            return await membermaxContext.Documents
                 .Where(x => x.StaffId == staffId)
                .FirstOrDefaultAsync();
        }

        public async Task<Document> GetPictureByEventIdAsync(int eventId)
        {
            return await membermaxContext.Documents
                 .Where(x => x.EventId == eventId && x.EventBannerImageId == null || x.EventBannerImageId == 0)
                .FirstOrDefaultAsync();
        }
        public async Task<Document> GetBannerByEventIdAsync(int eventId)
        {
            return await membermaxContext.Documents
                 .Where(x => x.EventId == eventId && x.EventBannerImageId == eventId)
                .FirstOrDefaultAsync();
        }

        public async Task<Document> GetImageByOrganizationIdAndTitleAsync(int organizationId, string title)
        {
            return await membermaxContext.Documents
                 .Where(x => x.OrganizationId == organizationId && x.Title == title)
                .SingleOrDefaultAsync();
        }

        public async Task<string> GetDocumentPathAsync(int entityId, int documentId)
        {
            var document = await membermaxContext.Documents
                            .Where(x => x.EntityId == entityId && x.DocumentId == documentId)
                            .SingleOrDefaultAsync();
            return document.FilePath;
        }
        public async Task<string> GetDocumentTitleAsync(int entityId, int documentId)
        {
            var document = await membermaxContext.Documents
                            .Where(x => x.EntityId == entityId && x.DocumentId == documentId)
                            .SingleOrDefaultAsync();
            return document.Title;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
