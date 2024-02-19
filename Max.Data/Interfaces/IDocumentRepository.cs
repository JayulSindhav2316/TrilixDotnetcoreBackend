using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IDocumentRepository : IRepository<Document>
    {
        Task<IEnumerable<Document>> GetAllDocumentsAsync();
        Task<Document> GetDocumentByIdAsync(int id);
        Task<IEnumerable<Document>> GetDocumentsByEntityIdAsync(int id);
        Task<Document> GetPictureByEntityIdAsync(int id);
        Task<Document> GetProfilePictureByStaffIdAsync(int staffId);
        Task<string> GetDocumentPathAsync(int personId, int docId);
        Task<string> GetDocumentTitleAsync(int personId, int docId);
        Task<Document> GetImageByOrganizationIdAndTitleAsync(int organizationId, string title);
        Task<Document> GetPictureByEventIdAsync(int eventId);
        Task<Document> GetBannerByEventIdAsync(int eventId);
    }
}
