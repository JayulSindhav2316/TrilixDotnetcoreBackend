using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Max.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<Document>> GetAllDocuments();
        Task<Document> GetDocumentById(int id);
        Task<Document> CreateDocument(IFormFile file, DocumentModel model);
        Task<IEnumerable<Document>> GetDocumentsByEntityId(int id);
        Task<Document> GetPictureByEntityId(int id);
        Task<DocumentModel> GetProfileImageById(string path,string tenantId,int id);
        Task<bool> DeleteProfileImageByEntityId(int id, string rootFolder);
        Task<DocumentModel> GetOrganizationImageByOrgIdAndTitle(string path, int id, string title);
        //Task<DocumentModel> CreatePdfInvoice(string rootPath, int id);
        //DocumentModel CreatePdfReceipt(string rootPath, ReceiptModel receiptModel);
        Task<DocumentModel> GetStaffProfileImageById(string documentRoot, string tenantId, int staffId);
        Task<bool> DeleteStaffProfileImageById(int staffId, string rootFolder, string tenantId);
        Task<DocumentModel> GetEventImageById(string documentRoot, string tenantId, int eventId);
        Task<DocumentModel> GetEventCoverImageById(string documentRoot, string tenantId, int eventId);
        Task<bool> DeleteEventImageById(int eventId, string rootFolder, string tenantId);
        Task<bool> DeleteEventBannerImageById(int eventId, string rootFolder, string tenantId);
        Task<bool> CloneEventImageById(int cloneEventId, int masterEventId);
        Task<bool> CloneEventCoverImageById(int cloneEventId, int masterEventId);
        Task<Document> CreateDocument(DocumentModel model);
    }
}
