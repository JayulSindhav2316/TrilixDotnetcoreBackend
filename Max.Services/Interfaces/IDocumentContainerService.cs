using Max.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IDocumentContainerService
    {

        Task<DocumentContainerModel> CreateDocumentContainer(ContainerRequestModel model);
        Task<DocumentObjectModel> CreateFolder(DocumentObjectModel model);
        Task<DocumentContainerModel> UpdateDocumentContainer(ContainerRequestModel model);
        Task<bool> DeleteDocumentContainer(ContainerRequestModel model);
        Task<List<ContainerTreeModel>> GetDocumentContainerTree(int? entityId,string node, string selectedNode);
        Task<DocumentObjectModel> CreateDocumentObject(IFormFile file, DocumentObjectModel model);
        Task<DocumentObjectResponseModel> GetDocumentObject(DocumentObjectRequestModel model);
        Task<DocumentObjectResponseModel> GetDocumentObjectUrl(DocumentObjectRequestModel model);
        Task<List<ContainerAccessModel>> GetContainerAccessListByGroupId(int groupId);
        Task<List<DocumentAccessModel>> GetDocumentAccessListByGroupId(int groupId);
        Task<List<ContainerAccessModel>> GetContainerAccessListByContainerId(int id);
        Task<List<DocumentContainerModel>> GetAllDocumentContainers();
        Task<DocumentContainerModel> GetDocumentContainerById(int id);
        Task<DocumentContainerModel> GetDocumentContainerByFolderKey(string key);
        Task<List<DocumentObjectModel>> GetDocumentObjectsByContainerAndPath(int containerId, string path, int? entityId);
        Task<SolrSearchResultModel> GetDocumentObjectsByText(string text, string filter, string tags, DateTime fromdate, DateTime toDate,int entityId, int startPage, string sortBy,int staffUserId, string tenantId);
        Task<bool> DeleteFolder(DocumentObjectModel model);
        Task<DocumentObjectModel> UpdateFolder(DocumentObjectModel model);
        Task<bool> DeleteDocument(DocumentObjectRequestModel model);
        Task<DocumentTagModel> AddTagToDocumentObject(DocumentTagModel model);
        Task<Boolean> RemoveTagFromDocumentObject(DocumentTagModel model);
        Task<List<DocumentTagModel>> GetTagsByDocumentObjectId(int id);
        Task<List<DocumentTagModel>> GetDocumentObjectsByTagId(int id);
        Task<List<DocumentObjectModel>> GetFoldersByContainerAndPath(int containerId, string filePath);
        Task<DocumentObjectModel> UploadLocalFile(string folder, string file, DocumentObjectModel model);
        Task<List<DocumentObjectAccessHistoryModel>> GetAuditTrailByDocumentId(int id);
        Task<List<DocumentObjectAccessHistoryModel>> GetAuditTrailByDateRange(DateTime startDate, DateTime endDate);
        Task<List<DocumentAccessModel>> GetDocumentAccessListByDocumentId(int id);
        Task<List<DocumentTagModel>> GetDocumentTagListById(int id);
        Task<DocumentObjectModel> UpdateDocumentAccessControl(DocumentObjectModel model);
        Task<DocumentObjectModel> UpdateDocumentTags(DocumentObjectModel model);
        Task<BarChartModel> GetSearchStatistics();
        Task<BarChartModel> GetMemberPortalActiveUsers();
        Task<bool> ExportDocuments(string userName);
        public bool UploadSolrDocument(SolrDocumentModel model);
    }
}
