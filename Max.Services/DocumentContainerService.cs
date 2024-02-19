using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using static Max.Core.Constants;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using Max.Services.Helpers;
using Microsoft.AspNetCore.StaticFiles;
using iTextSharp.text.pdf;
using Max.Core.Helpers;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Max.Services
{

    public class DocumentContainerService : IDocumentContainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        static readonly ILogger _logger = Serilog.Log.ForContext<DocumentContainerService>();
        private readonly IMapper _mapper;
        private readonly IAzureStorageService _azureStorageService;
        private readonly ISolrIndexService<SolrDocumentModel> _solrIndexService;
        private readonly IMemoryCache _memoryCache;

        public DocumentContainerService(IUnitOfWork unitOfWork, IMapper mapper, IAzureStorageService azureStorageService, ISolrIndexService<SolrDocumentModel> solrIndexService, IMemoryCache memoryCache)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._azureStorageService = azureStorageService;
            this._solrIndexService = solrIndexService;
            this._memoryCache = memoryCache;
        }

        public async Task<DocumentContainerModel> CreateDocumentContainer(ContainerRequestModel model)
        {
            _logger.Information($"Creating Document Container Name:{model.Name}.");

            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByNameAsync(model.Name, model.ContainerId);
            var accessControlConfiguration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(model.OrganizationId);

            if (container != null)
            {
                _logger.Information($"A container already exists with Name :{model.Name}.");
                throw new Exception($"A container already exists with Name :{model.Name}.");
            }

            container = new Documentcontainer();

            container.Description = model.Description;
            container.Name = model.Name;
            container.CreatedDate = DateTime.Now;
            container.CreatedBy = model.UserId;
            container.AccessControlEnabled = model.AccessControlEnabled;
            container.EntityId = model.EntityId;    
            if (model.AccessControlEnabled == (int)Status.Active)
            {
                if (accessControlConfiguration.DocumentAccessControl == DocumentAccessControl.MEMBERSHIP)
                {
                    if (model.MembershipTypes.IsNullOrEmpty())
                    {
                        _logger.Information($"No access control defined for container:{model.Name}.");
                        throw new Exception($"No access control defined for container:{model.Name}.");
                    }
                    foreach (var type in model.MembershipTypes)
                    {
                        var containerAccess = new Containeraccess();

                        containerAccess.MembershipTypeId = type;
                        container.Containeraccesses.Add(containerAccess);
                    }
                }
                else if (accessControlConfiguration.DocumentAccessControl == DocumentAccessControl.GROUP)
                {
                    if (model.Groups.IsNullOrEmpty() && model.StaffRoles.IsNullOrEmpty())
                    {
                        _logger.Information($"No access control defined for container:{model.Name}.");
                        throw new Exception($"No access control defined for container:{model.Name}.");
                    }
                    foreach (var group in model.Groups)
                    {
                        var containerAccess = new Containeraccess();

                        containerAccess.GroupId = group;
                        container.Containeraccesses.Add(containerAccess);
                    }
                }
                if (model.StaffRoles.Count > 0)
                {
                    foreach (var role in model.StaffRoles)
                    {
                        var containerAccess = new Containeraccess();
                        containerAccess.StaffRoleId = role;
                        container.Containeraccesses.Add(containerAccess);
                    }
                }
            }

            container.BlobContainerId = GetObjectId();
            container.EncryptionKey = GetEncryptionKey();
            try
            {
                await _unitOfWork.DocumentContainers.AddAsync(container);
                await _unitOfWork.CommitAsync();
                _logger.Information($"Document Container created with Name :{model.Name}.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Document Container creation failed with Name :{model.Name}.");
                throw new Exception($"Document Container creation failed with Name :{model.Name}.");
            }

            return _mapper.Map<DocumentContainerModel>(container);

        }

        public async Task<DocumentObjectModel> CreateFolder(DocumentObjectModel model)
        {
            _logger.Information($"Creating Document Container Name:{model.FileName}.");

            var folder = await _unitOfWork.DocumentObjects.GetDocumentObjectByContainerIdAndNameAsync(model.ContainerId, model.PathName, model.FileName);

            if (folder != null)
            {
                _logger.Information($"A folder already exists with Name :{model.FileName}.");
                throw new Exception($"A folder already exists with Name :{model.FileName}.");
            }

            folder = new Documentobject();

            folder.ContainerId = model.ContainerId;
            folder.FileName = model.FileName;
            folder.PathName = model.PathName;
            folder.FileType = (int)DocumentObjectType.Folder;
            folder.CreatedBy = model.CreatedBy;
            folder.CreatedDate = DateTime.Now;
            folder.Active = (int)Status.Active;

            try
            {
                await _unitOfWork.DocumentObjects.AddAsync(folder);
                await _unitOfWork.CommitAsync();
                _logger.Information($"Folder created with Name :{model.FileName}.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Folder creation failed with Name :{model.FileName}.");
                throw new Exception($"Folder creation failed with Name :{model.FileName}.");
            }

            return _mapper.Map<DocumentObjectModel>(folder);
        }

        public async Task<DocumentObjectModel> CreateDocumentObject(IFormFile file, DocumentObjectModel model)
        {
            string documentText = string.Empty;
            var documentObject = new DocumentObjectModel();
            var existingDocumentObject = await _unitOfWork.DocumentObjects.GetActiveDocumentObjectByNameAsync(model.FileName, model.PathName);
            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByIdAsync(model.ContainerId);
            if (existingDocumentObject == null)
            {
                Documentobject document = new Documentobject();
                document.ContainerId = model.ContainerId;
                document.CreatedBy = model.CreatedBy;
                document.CreatedDate = DateTime.Now;
                document.FileType = model.FileType;
                document.PathName = model.PathName;
                document.FileName = model.FileName;
                document.BlobId = GetObjectId();
                if(model.entityId > 0)
                {
                    document.EntityId = model.entityId;
                }
                //Upload file to Azure blob storage

                try
                {
                    //Save file in temp folder

                    var tempFolderPath = Path.Combine($"{model.TempRootFolder}/Documents/{model.TenantId}/Organization/Temp/" + model.ContainerId);
                    var mappedPath = Path.GetFullPath(tempFolderPath);
                    if (!Directory.Exists(mappedPath))
                    {
                        Directory.CreateDirectory(mappedPath);
                    }
                    string inputFile = Path.GetFullPath(Path.Combine(mappedPath, model.FileName));
                    string outputFile = Path.GetFullPath(Path.Combine(mappedPath, "Temp_" + model.FileName));
                    using (var stream = new FileStream(inputFile, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    if (inputFile.EndsWith(".pdf"))
                    {
                        PdfReader reader = new PdfReader(inputFile);
                        if (reader.Info["CreationDate"] != null)
                        {
                            string creationDate = reader.Info["CreationDate"].ToString();
                            if (!creationDate.IsNullOrEmpty())
                            {
                                document.CreatedDate = CreateDateTime(creationDate);
                            }
                        }

                    }

                    CryptoUtil.EncryptFile(inputFile, outputFile, container.EncryptionKey);

                    var memory = new MemoryStream();

                    MemoryStream blob = new MemoryStream();
                    using (var stream = new FileStream(outputFile, FileMode.Open))
                    {
                        await stream.CopyToAsync(blob);
                    }
                    blob.Position = 0;
                    var size = await _azureStorageService.CreateBlob(model.OrganizationId, container.BlobContainerId, document.BlobId, blob);

                    documentText = _solrIndexService.ExtractDocumentText(inputFile);

                    _logger.Information($"Updated Index for {inputFile}.");

                    //Remove temp files
                    File.Delete(inputFile);
                    File.Delete(outputFile);
                    document.FileSize = size;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to upload file to Azure storage:{model.FileName}.");
                    throw new Exception($"Failed to upload file to Azure storage:{model.FileName}.");
                }

                try
                {
                    //Check if tags are included

                    if (model.SelectedTags.Count > 0)
                    {
                        foreach (var tag in model.SelectedTags)
                        {
                            if (tag == null)
                            {
                                continue;
                            }
                            Documenttag documentTag = new Documenttag();
                            documentTag.TagId = Int32.Parse(tag.code);
                            document.Documenttags.Add(documentTag);
                        }
                    }

                    document.Active = (int)Status.Active;
                    await _unitOfWork.DocumentObjects.AddAsync(document);
                    await _unitOfWork.CommitAsync();
                    _logger.Information($"Folder created with Name :{model.FileName}.");
                    documentObject = _mapper.Map<DocumentObjectModel>(document);

                    // Add it to Solr Indexer
                    //SolrDocumentModel solrDocument = new SolrDocumentModel();
                    //var staffUser = await _unitOfWork.Staffusers.GetByIdAsync(model.CreatedBy);
                    //solrDocument.Id = $"{model.TenantId}-{documentObject.DocumentObjectId}";
                    //solrDocument.Text = documentText;
                    //solrDocument.TenantId = model.TenantId;
                    //solrDocument.CreatedBy = staffUser.UserName;
                    //solrDocument.CreatedDate = (DateTime)documentObject.CreatedDate;
                    //solrDocument.FileName = documentObject.FileName;
                    //_solrIndexService.AddUpdate(solrDocument);

                }
                catch (Exception ex)
                {
                    _logger.Error($"File creation failed with Name :{model.FileName}. Error {ex.Message}");
                    throw new Exception($"File creation failed with Name :{model.FileName}.");
                }
            }
            else
            {
                _logger.Error($"A document already exists with name :{model.PathName}/{model.FileName}.");
                throw new Exception($"A document already exists with name :{model.PathName}/{model.FileName}.");
            }
            return documentObject;
        }

        public async Task<DocumentObjectModel> UploadLocalFile(string folder, string file, DocumentObjectModel model)
        {
            string documentText = string.Empty;
            var documentObject = new DocumentObjectModel();
            var existingDocumentObject = await _unitOfWork.DocumentObjects.GetDocumentObjectByNameAsync(model.FileName, model.PathName);
            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByIdAsync(model.ContainerId);
            if (existingDocumentObject == null)
            {
                Documentobject document = new Documentobject();
                document.ContainerId = model.ContainerId;
                document.CreatedBy = model.CreatedBy;
                document.CreatedDate = DateTime.Now;
                document.FileType = model.FileType;
                document.PathName = model.PathName;
                document.FileName = model.FileName;
                document.BlobId = GetObjectId();

                //Upload file to Azure blob storage

                try
                {
                    //Save file in temp folder

                    var tempFolderPath = Path.Combine($"{folder}/Temp/" + model.ContainerId);
                    var mappedPath = Path.GetFullPath(tempFolderPath);
                    if (!Directory.Exists(mappedPath))
                    {
                        Directory.CreateDirectory(mappedPath);
                    }
                    string inputFile = file;
                    string outputFile = Path.GetFullPath(Path.Combine(mappedPath, "Temp_" + model.FileName));


                    CryptoUtil.EncryptFile(inputFile, outputFile, container.EncryptionKey);

                    var memory = new MemoryStream();

                    MemoryStream blob = new MemoryStream();
                    using (var stream = new FileStream(outputFile, FileMode.Open))
                    {
                        await stream.CopyToAsync(blob);
                    }
                    blob.Position = 0;
                    var size = await _azureStorageService.CreateBlob(model.OrganizationId, container.BlobContainerId, document.BlobId, blob);

                    documentText = _solrIndexService.ExtractDocumentText(inputFile);

                    _logger.Information($"Updated Index for {inputFile}.");

                    //Remove temp files
                    //File.Delete(inputFile);
                    File.Delete(outputFile);
                    document.FileSize = size;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to upload file to Azure storage:{model.FileName}.");
                    throw new Exception($"Failed to upload file to Azure storage:{model.FileName}.");
                }

                try
                {
                    //Check if tags are included

                    if (model.SelectedTags.Count > 0)
                    {
                        foreach (var tag in model.SelectedTags)
                        {
                            if (tag == null)
                            {
                                continue;
                            }
                            Documenttag documentTag = new Documenttag();
                            documentTag.TagId = Int32.Parse(tag.code);
                            documentTag.TagValue = tag.name;
                            document.Documenttags.Add(documentTag);
                        }
                    }

                    //Check if Access at document level is defined
                    if (container.AccessControlEnabled == (int)Status.Active)
                    {
                        var accessGroups = model.AccessList.Split(",");
                        if (accessGroups.Length > 0)
                        {
                            var containerAccess = await _unitOfWork.ContainerAccesses.GetContainerAccessByContainerIdAsync(container.ContainerId);
                            if (accessGroups.Length != containerAccess.Count())
                            {
                                foreach (var accessItem in containerAccess)
                                {
                                    if (accessGroups.Contains(accessItem.GroupId.ToString()))
                                    {
                                        var documentaccess = new Documentaccess();

                                        documentaccess.GroupId = accessItem.GroupId;
                                        document.Documentaccesses.Add(documentaccess);
                                    }
                                }
                            }
                        }
                    }
                    document.Active = (int)Status.Active;
                    await _unitOfWork.DocumentObjects.AddAsync(document);
                    await _unitOfWork.CommitAsync();
                    _logger.Information($"Folder created with Name :{model.FileName}.");
                    documentObject = _mapper.Map<DocumentObjectModel>(document);

                    // Add it to Solr Indexer

                    SolrDocumentModel solrDocument = new SolrDocumentModel();

                    solrDocument.Id = $"{model.TenantId}-{documentObject.DocumentObjectId}";
                    solrDocument.Text = documentText;
                    solrDocument.TenantId = model.TenantId;
                    solrDocument.Text = documentText;
                    solrDocument.CreatedBy = "System";
                    solrDocument.CreatedDate = (DateTime)documentObject.CreatedDate;
                    solrDocument.FileName = documentObject.FileName;
                    _solrIndexService.AddUpdate(solrDocument);

                }
                catch (Exception ex)
                {
                    _logger.Error($"File creation failed with Name :{model.FileName}. Error {ex.Message}");
                    throw new Exception($"File creation failed with Name :{model.FileName}.");
                }
            }
            else
            {
                _logger.Error($"A document already exists with name :{model.PathName}/{model.FileName}.");
                throw new Exception($"A document already exists with name :{model.PathName}/{model.FileName}.");
            }
            return documentObject;
        }

        public async Task<DocumentObjectResponseModel> GetDocumentObject(DocumentObjectRequestModel model)
        {
            var documentObject = new DocumentObjectResponseModel();
            var document = await _unitOfWork.DocumentObjects.GetDocumentObjectByIdAsync(model.DocumentObjectId);
            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByIdAsync(document.ContainerId ?? 0);
            _logger.Information($"GetDocumentObject: Document Access:Document Id:{model.DocumentObjectId} organization Id:{model.OrganizationId} StaffId:{model.StaffId} EntityId: {model.EntityId}");
            if (document != null)
            {
                if (!string.IsNullOrEmpty(document.BlobId))
                {
                    _logger.Information($"GetDocumentObject: Checking  Access for :Document Id:{model.DocumentObjectId}");
                    //Check the container access
                    var containerAccess = await _unitOfWork.ContainerAccesses.GetContainerAccessByContainerIdAsync(document.ContainerId ?? 0);
                    var documentAccess = await _unitOfWork.DocumentObjectAccesses.GetDocumentAccessByDocumentObjectIdAsync(model.DocumentObjectId);
                    try
                    {
                        _logger.Information($"GetDocumentObject: containerAccess count {containerAccess.ToList().Count} :Document Id:{model.DocumentObjectId}");
                        if (containerAccess.ToList().Count > 0 && model.StaffId == 0)
                        {
                            var accessList = await _unitOfWork.Groups.GetAllGroupsByEntityIdAsync(model.EntityId);
                            foreach (var accessItem in accessList.ToList())
                            {
                                _logger.Information($"GetDocumentObject: AccessList for entity {model.EntityId} -> {accessItem.GroupId}");
                            }

                            var documentAccessList = documentAccess.Select(x => x.GroupId).ToList();
                            var containerAccessList = containerAccess.Select(x => x.GroupId).ToList();

                            //If Documentlevel access is defined then first check that else Go for Container level access check
                            if (documentAccessList.Count() > 0)
                            {
                                if (!accessList.Any(x => documentAccessList.Contains(x.GroupId)))
                                {
                                    _logger.Information($"GetDocumentObject: Access not allowed for document access for entity {model.EntityId} :Document Id:{model.DocumentObjectId}");
                                    documentObject.ResponseMessage = "Access Denied";
                                    return documentObject;
                                }
                            }
                            else
                            {
                                if (!accessList.Any(x => containerAccessList.Contains(x.GroupId)))
                                {
                                    _logger.Information($"GetDocumentObject: access not allowed for container  entity {model.EntityId} :Document Id:{model.DocumentObjectId}");
                                    documentObject.ResponseMessage = "Access Denied";
                                    return documentObject;
                                }
                            }
                        }
                        else
                        {
                            var roleAccess = await _unitOfWork.Staffusers.GetStaffUserByIdAsync(model.StaffId);
                            if (documentAccess.ToList().Count > 0 || containerAccess.ToList().Count > 0)
                            {
                                try
                                {
                                    var documentAccessList = documentAccess.Select(x => x.StaffRoleId).ToList();
                                    var containerAccessList = containerAccess.Select(x => x.StaffRoleId).ToList();
                                    //if (containerAccess.ToList().Count > 0)
                                    //{
                                    if (documentAccessList.Count() > 0)
                                    {
                                        if (!roleAccess.Staffroles.Any(x => documentAccessList.ToList().Contains(x.RoleId)))
                                        {
                                            _logger.Information($"GetDocumentObject: Access not allowed for document access for entity {model.EntityId} :Document Id:{model.DocumentObjectId}");
                                            documentObject.ResponseMessage = "Access Denied";
                                            return documentObject;
                                        }
                                    }
                                    else
                                    {
                                        //if (!containerAccessList.ToList().Any(x => x.StaffRoleId == roleAccess.Staffroles.FirstOrDefault().RoleId))
                                        if (!roleAccess.Staffroles.Any(x => containerAccessList.ToList().Contains(x.RoleId)))
                                        {
                                            _logger.Information($"GetDocumentObject: access not allowed for container  Staff user {model.StaffId} :Document Id:{model.DocumentObjectId}");
                                            documentObject.ResponseMessage = "Access Denied";
                                            return documentObject;
                                        }
                                    }
                                    //}

                                }

                                catch (Exception ex)
                                {

                                    throw;
                                }
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        _logger.Error($"Failed to download file :{document.FileName} Error Message {ex.Message} {ex.InnerException.Message} {ex.StackTrace}");

                    }
                    _logger.Information($"GetDocumentObject: Access Allowed for entity {model.EntityId} :Document Id:{model.DocumentObjectId}");
                    //Download file from Azure blob storage
                    if (document.Active == (int)Status.InActive)
                    {
                        //Deleted documents do not exists in Azure/Solr
                        return documentObject;
                    }
                    try
                    {
                        documentObject.DocumentName = document.FileName;
                        documentObject.DocumentType = GetContentType(document.FileName);
                        //Save file in temp folder

                        var tempFolderPath = Path.Combine($"{model.TempRootFolder}/Documents/{model.TenantId}/Organization/Temp/" + container.ContainerId);
                        var mappedPath = Path.GetFullPath(tempFolderPath);
                        if (!Directory.Exists(mappedPath))
                        {
                            Directory.CreateDirectory(mappedPath);
                        }
                        string downloadFile = Path.GetFullPath(Path.Combine(mappedPath, "Temp_" + document.FileName));

                        string outputFile = Path.GetFullPath(Path.Combine(mappedPath, document.FileName));

                        _logger.Information($"GetDocumentObject: Downloading file from Azure {container.BlobContainerId} :Document Id:{document.BlobId}");

                        await _azureStorageService.DownloadBlob(model.OrganizationId, container.BlobContainerId, document.BlobId, downloadFile);

                        _logger.Information($"GetDocumentObject: File downloaded from Azure {container.BlobContainerId} :Document Id:{document.BlobId}");

                        var result = CryptoUtil.DecryptFile(downloadFile, outputFile, container.EncryptionKey);
                        if(result != (int)FileDownloadStatus.Successful)
                        {
                            _logger.Information($"GetDocumentObject: File Decryption failed for :Document Id:{document.BlobId} Error: {result}");
                            throw new Exception($"Failed to download file from Azure storage:{document.FileName}.");
                        }

                        var memory = new MemoryStream();

                        MemoryStream blob = new MemoryStream();
                        using (var stream = new FileStream(outputFile, FileMode.Open))
                        {
                            await stream.CopyToAsync(blob);
                        }
                        blob.Position = 0;

                        documentObject.Document = blob.ToArray();

                        //Remove temp files
                        File.Delete(downloadFile);
                        File.Delete(outputFile);

                        //Insert entry in access history
                        _logger.Information($"GetDocumentObject: Adding access history {model.EntityId}");
                        if (model.EntityId > 0 || model.StaffId > 0)
                        {
                            var documentObjectAccessHistory = new Documentobjectaccesshistory();
                            documentObjectAccessHistory.DocumentObjectId = model.DocumentObjectId;
                            documentObjectAccessHistory.AccessDate = DateTime.Now;
                            if (model.EntityId > 0)
                            {
                                documentObjectAccessHistory.EntityId = model.EntityId;
                            }
                            else if (model.StaffId > 0)
                            {
                                documentObjectAccessHistory.StaffUserId = model.StaffId;
                            }
                            await _unitOfWork.DocumentObjectAccessHistories.AddAsync(documentObjectAccessHistory);
                            await _unitOfWork.CommitAsync();
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Failed to download file from Azure storage:{document.FileName} Error Message {ex.Message} {ex.InnerException.Message} {ex.StackTrace}");
                        throw new Exception($"Failed to download file from Azure storage:{document.FileName}.");
                    }
                }
                else
                {
                    _logger.Error($"A document does not exists with Id :{model.DocumentObjectId}.");
                    documentObject.ResponseMessage = "A blob of document does not exist";
                }
            }
            else
            {
                _logger.Error($"A document does not exists with Id :{model.DocumentObjectId}.");
                throw new Exception($"A document does not exists with Id :{model.DocumentObjectId}.");
            }
            return documentObject;
        }

        public async Task<DocumentObjectResponseModel> GetDocumentObjectUrl(DocumentObjectRequestModel model)
        {
            var documentObject = new DocumentObjectResponseModel();
            var document = await _unitOfWork.DocumentObjects.GetDocumentObjectByIdAsync(model.DocumentObjectId);
            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByIdAsync(document.ContainerId ?? 0);
            _logger.Information($"GetDocumentObjectUrl: Document Access:Document Id:{model.DocumentObjectId} organization Id:{model.OrganizationId} StaffId:{model.StaffId} EntityId: {model.EntityId}");
            if (document != null)
            {
                //Upload file to Azure blob storage

                try
                {
                    documentObject.DocumentName = document.FileName;
                    documentObject.DocumentType = GetContentType(document.FileName);
                    //Save file in temp folder

                    var tempFolderPath = Path.Combine($"{model.TempRootFolder}/Documents/{model.TenantId}/Organization/Temp/" + container.ContainerId);
                    var mappedPath = Path.GetFullPath(tempFolderPath);
                    if (!Directory.Exists(mappedPath))
                    {
                        Directory.CreateDirectory(mappedPath);
                    }
                    string downloadFile = Path.GetFullPath(Path.Combine(mappedPath, "Temp_" + document.FileName));

                    string outputFile = Path.GetFullPath(Path.Combine(mappedPath, document.FileName));

                    await _azureStorageService.DownloadBlob(model.OrganizationId, container.BlobContainerId, document.BlobId, downloadFile);

                    CryptoUtil.DecryptFile(downloadFile, outputFile, container.EncryptionKey);

                    //Remove temp files
                    File.Delete(downloadFile);
                    documentObject.DocumentUrl = outputFile;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to upload file to Azure storage:{document.FileName}.");
                    throw new Exception($"Failed to upload file to Azure storage:{document.FileName}.");
                }
            }
            else
            {
                _logger.Error($"A document down not exists with Id :{model.DocumentObjectId}.");
                throw new Exception($"A document down not exists with Id :{model.DocumentObjectId}.");
            }
            return documentObject;
        }

        public async Task<DocumentContainerModel> UpdateDocumentContainer(ContainerRequestModel model)
        {
            _logger.Information($"Updating  Document Container Id:{model.ContainerId} Name:{model.Name}.");

            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByIdAsync(model.ContainerId);
            var containerByName = await _unitOfWork.DocumentContainers.GetDocumentContainerByNameAsync(model.Name, model.ContainerId);
            var accessControlConfiguration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(model.OrganizationId);

            if (container == null)
            {
                _logger.Information($"Could not find a container with Id{model.ContainerId}.");
                throw new Exception($"Could not find a container with Id{model.ContainerId}.");
            }
            if (containerByName != null)
            {
                _logger.Information($"A container already exists with Name :{model.Name}.");
                throw new Exception($"A container already exists with Name :{model.Name}.");
            }


            //If Container name has changed then we need to update the documentObjects as well

            var needToUpdateDocumentObjects = container.Name != model.Name ? true : false;

            if (needToUpdateDocumentObjects)
            {
                var childObjects = await _unitOfWork.DocumentObjects.GetChildDocumentObjectsByContainerIdAndPathNameAsync(model.ContainerId, container.Name);

                if (!childObjects.IsNullOrEmpty())
                {
                    foreach (var childObject in childObjects)
                    {
                        childObject.PathName = childObject.PathName.ReplaceFirst(container.Name, model.Name);
                        _unitOfWork.DocumentObjects.Update(childObject);
                    }
                }
            }

            container.Description = model.Description;
            container.Name = model.Name;

            //Remove old AccessList if it has been disabled
            if (container.AccessControlEnabled == (int)Status.Active && model.AccessControlEnabled == (int)Status.InActive)
            {
                var currentAccessList = container.Containeraccesses.ToList();
                foreach (var accessControlItem in currentAccessList)
                {
                    container.Containeraccesses.Remove(accessControlItem);
                }
            }

            container.AccessControlEnabled = model.AccessControlEnabled;

            if (model.AccessControlEnabled == (int)Status.Active)
            {
                if (accessControlConfiguration.DocumentAccessControl == DocumentAccessControl.MEMBERSHIP)
                {
                    if (model.MembershipTypes.IsNullOrEmpty())
                    {
                        _logger.Information($"No access control defined for container:{model.Name}.");
                        throw new Exception($"No access control defined for container:{model.Name}.");
                    }

                    //Remove unselcted
                    var currentAccessList = container.Containeraccesses.ToList();
                    foreach (var accessControlItem in currentAccessList)
                    {
                        if (!model.MembershipTypes.Contains(accessControlItem.MembershipTypeId ?? 0))
                        {
                            container.Containeraccesses.Remove(accessControlItem);
                        }
                    }

                    //Add new
                    foreach (var type in model.MembershipTypes)
                    {
                        if (!container.Containeraccesses.Any(x => x.MembershipTypeId == type))
                        {
                            var containerAccess = new Containeraccess();
                            containerAccess.MembershipTypeId = type;
                            container.Containeraccesses.Add(containerAccess);
                        }

                    }
                }
                else
                {
                    if (model.Groups.IsNullOrEmpty() && model.StaffRoles.IsNullOrEmpty())
                    {
                        _logger.Information($"No access control defined for container:{model.Name}.");
                        throw new Exception($"No access control defined for container:{model.Name}.");
                    }

                    //Remove unselcted
                    var currentAccessList = container.Containeraccesses.ToList();
                    foreach (var accessControlItem in currentAccessList)
                    {

                        var test = model.Groups.Contains(accessControlItem.GroupId ?? 0);
                        if (!model.Groups.Contains(accessControlItem.GroupId ?? 0) && accessControlItem.StaffRoleId == null)
                        {
                            //if(accessControlItem.StaffUserId==null)
                            container.Containeraccesses.Remove(accessControlItem);
                            //Remove from DocumentObjects

                            var documentObjects = await _unitOfWork.DocumentObjectAccesses.GetDocumentObjectAccessListByGroupIdAsync(accessControlItem.GroupId ?? 0);
                            if (documentObjects.Count() > 0)
                            {
                                _unitOfWork.DocumentObjectAccesses.RemoveRange(documentObjects);
                            }

                        }
                    }



                    //Add new
                    foreach (var type in model.Groups)
                    {
                        if (!container.Containeraccesses.Any(x => x.GroupId == type))
                        {
                            var containerAccess = new Containeraccess();
                            containerAccess.GroupId = type;
                            container.Containeraccesses.Add(containerAccess);
                        }

                    }
                }

                // Remove unselcted
                var currentList = container.Containeraccesses.ToList();
                foreach (var accessControlItem in currentList)
                {
                    //var test = model.StaffRoles.Contains(accessControlItem.StaffUserId ?? 0);
                    if (!model.StaffRoles.Contains(accessControlItem.StaffRoleId ?? 0) && accessControlItem.StaffRoleId != null)
                    {
                        if (accessControlItem.GroupId == null)
                            container.Containeraccesses.Remove(accessControlItem);
                        //Remove from DocumentObjects

                        var documentObjects = await _unitOfWork.DocumentObjectAccesses.GetDocumentObjectAccessListByStaffRoleIdAsync(accessControlItem.StaffRoleId ?? 0);
                        if (documentObjects.Count() > 0)
                        {
                            _unitOfWork.DocumentObjectAccesses.RemoveRange(documentObjects);
                        }

                    }
                }

                //add new
                foreach (var role in model.StaffRoles)
                {
                    if (!container.Containeraccesses.Any(x => x.StaffRoleId == role))
                    {
                        var containerAccess = new Containeraccess();
                        containerAccess.StaffRoleId = role;
                        container.Containeraccesses.Add(containerAccess);
                    }

                }



            }

            try
            {
                _unitOfWork.DocumentContainers.Update(container);
                await _unitOfWork.CommitAsync();
                _logger.Information($"Document Container updated with Name :{model.Name}.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Document Container updation failed with Name :{model.Name}.");
                throw new Exception($"Document Container updation failed with Name :{model.Name}.");
            }

            return _mapper.Map<DocumentContainerModel>(container);

        }

        public async Task<bool> DeleteDocumentContainer(ContainerRequestModel model)
        {
            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByIdAsync(model.ContainerId);

            if (container == null)
            {
                _logger.Information($"Could not find a container with Id{model.ContainerId}.");
                throw new Exception($"Could not find a container with Id{model.ContainerId}.");
            }
            //checkif it has child objects
            var doocumentObjects = await _unitOfWork.DocumentObjects.GetAllDocumentObjectsByContainerIdAsync(container.ContainerId);

            //checkif it has sub folders
            var containerSubFolders = await _unitOfWork.DocumentObjects.GetSubFoldersByContainerIdAsync(container.ContainerId);

            if (!doocumentObjects.IsNullOrEmpty() || !containerSubFolders.IsNullOrEmpty())
            {
                _logger.Information($"Container can not be deleted as it is not empty.");
                throw new Exception($"Container can not be deleted as it is not empty.");
            }

            try
            {
                _unitOfWork.DocumentContainers.Remove(container);
                await _unitOfWork.CommitAsync();
                _logger.Information($"Document Container deleted with Name :{container.Name}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Document Container deletion failed with Name :{container.Name}.");
                throw new Exception($"Document Container updation failed with Name :{container.Name}.");
            }
        }

        public async Task<DocumentObjectModel> CreateDocumentObject(DocumentObjectModel model)
        {
            _logger.Information($"Creating Document Object Container:{model.ContainerId} Name:{model.FileName}.");

            var documentObject = await _unitOfWork.DocumentObjects.GetDocumentObjectByContainerIdAndNameAsync(model.ContainerId, model.PathName, model.FileName);

            if (documentObject != null)
            {
                string errorMessage = $"A document already exists in the current directory :{model.FileName}.";
                _logger.Information(errorMessage);
                throw new Exception(errorMessage);
            }

            documentObject = _mapper.Map<Documentobject>(model);

            try
            {
                await _unitOfWork.DocumentObjects.AddAsync(documentObject);
                await _unitOfWork.CommitAsync();
                _logger.Information($"Document Object created with Name :{model.FileName}.");
            }
            catch (Exception ex)
            {
                string errorMessage = $"Failed to create the document :{model.FileName}.";
                _logger.Information(errorMessage);
                throw new Exception(errorMessage);
            }

            return _mapper.Map<DocumentObjectModel>(documentObject);
        }

        public async Task<List<DocumentContainerModel>> GetAllDocumentContainers()
        {
            var documentContainer = new List<DocumentContainerModel>();

            var containers = await _unitOfWork.DocumentContainers.GetAllDocumentContainersAsync();

            foreach (var container in containers)
            {
                var containerModel = new DocumentContainerModel();

                containerModel.ContainerId = container.ContainerId;
                containerModel.Name = container.Name;
                containerModel.Description = container.Description;
                containerModel.CreatedDate = (DateTime)container.CreatedDate;
                containerModel.AccessControlEnabled = container.AccessControlEnabled;
                if (container.AccessControlEnabled == (int)Status.Active)
                {
                    foreach (var accessItem in container.Containeraccesses)
                    {
                        var containerAccess = new ContainerAccessModel();

                        containerAccess.ContainerAccessId = accessItem.ContainerAccessId;
                        containerAccess.ContainerId = container.ContainerId;
                        if (accessItem.MembershipTypeId != null)
                        {
                            containerAccess.MembershipTypeId = accessItem.MembershipTypeId ?? 0;
                            containerAccess.MembershipCategoryId = accessItem.MembershipType.Category ?? 0;

                            var membershipType = new MembershipTypeModel();
                            membershipType.MembershipTypeId = accessItem.MembershipTypeId ?? 0;
                            membershipType.Name = accessItem.MembershipType.Name;
                            membershipType.CategoryName = accessItem.MembershipType.CategoryNavigation.Name;
                            membershipType.PeriodName = accessItem.MembershipType.PeriodNavigation.Name;

                            containerAccess.MembershipType = membershipType;

                            containerModel.ContainerAccesss.Add(containerAccess);
                        }
                        else if (accessItem.GroupId != null)
                        {
                            containerAccess.GroupId = accessItem.GroupId ?? 0;
                            containerAccess.GroupName = accessItem.Group.GroupName;
                            containerAccess.GroupDescription = accessItem.Group.GroupDescription;
                            containerModel.ContainerAccesss.Add(containerAccess);
                        }
                        if (accessItem.StaffRoleId != null)
                        {
                            containerAccess.StaffRoles = accessItem.StaffRoleId ?? 0;
                            containerAccess.GroupName = accessItem.StaffRole.Name;
                            containerAccess.GroupDescription = accessItem.StaffRole.Description;
                            containerModel.ContainerAccesss.Add(containerAccess);
                        }

                    }
                }
                documentContainer.Add(containerModel);
            }

            return documentContainer;

        }

        public async Task<DocumentContainerModel> GetDocumentContainerById(int id)
        {
            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByIdAsync(id);

            var documentContainer = _mapper.Map<DocumentContainerModel>(container);

            if (documentContainer.AccessControlEnabled == (int)Status.Active)
            {
                foreach (var accessItem in container.Containeraccesses)
                {
                    var containerAccess = new ContainerAccessModel();

                    containerAccess.ContainerAccessId = accessItem.ContainerAccessId;
                    if (accessItem.MembershipTypeId != null)
                    {
                        containerAccess.MembershipTypeId = accessItem.MembershipTypeId ?? 0;
                        containerAccess.MembershipCategoryId = accessItem.MembershipType.Category ?? 0;
                        containerAccess.GroupId = 0;
                    }
                    else if (accessItem.GroupId != null)
                    {
                        containerAccess.GroupId = accessItem.GroupId ?? 0;
                        containerAccess.MembershipTypeId = 0;
                    }
                    if (accessItem.StaffRoleId != null)
                    {
                        containerAccess.StaffRoles = accessItem.StaffRoleId ?? 0;
                        //containerAccess.StaffRoles = 0;
                    }
                    documentContainer.ContainerAccesss.Add(containerAccess);
                }
            }
            return documentContainer;
        }

        public async Task<DocumentContainerModel> GetDocumentContainerByFolderKey(string key)
        {

            int containerId = 0;
            int documentObjectId = 0;

            //Key structure => containerId-documentObjectId:file path

            if (key.IndexOf(':') > 0)
            {
                var values = key.Split(":");
                if (values[0] != null)
                {
                    var keys = values[0].Split("-");
                    containerId = int.Parse(keys[0]);
                    documentObjectId = int.Parse(keys[1]);
                }
            }
            else
            {
                containerId = int.Parse(key);
            }

            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByIdAsync(containerId);

            var documentContainer = _mapper.Map<DocumentContainerModel>(container);

            if (documentContainer.AccessControlEnabled == (int)Status.Active)
            {
                foreach (var accessItem in container.Containeraccesses)
                {
                    var containerAccess = new ContainerAccessModel();

                    containerAccess.ContainerAccessId = accessItem.ContainerAccessId;
                    if (accessItem.MembershipTypeId != null)
                    {
                        containerAccess.MembershipTypeId = accessItem.MembershipTypeId ?? 0;
                        containerAccess.MembershipCategoryId = accessItem.MembershipType.Category ?? 0;
                        containerAccess.GroupId = 0;
                    }
                    else if (accessItem.GroupId != null)
                    {
                        containerAccess.GroupId = accessItem.GroupId ?? 0;
                        containerAccess.MembershipTypeId = 0;
                    }
                    documentContainer.ContainerAccesss.Add(containerAccess);
                }
            }
            return documentContainer;
        }

        public async Task<List<ContainerTreeModel>> GetDocumentContainerTree(int? entityId,string root = "/", string selectedNode = "")
        {
            var containerTree = new List<ContainerTreeModel>();
            int selectedContainer = 0;
            string selectedFolder = string.Empty;

            if (!selectedNode.IsNullOrEmpty())
            {
                string[] keys = selectedNode.Split("-");
                selectedContainer = int.Parse(keys[0]);

                //Extract Path
                string[] path = selectedNode.Split(":");
                if (path.Length > 0)
                {
                    selectedFolder = path[1];
                }
            }

             var containers = await _unitOfWork.DocumentContainers.GetAllDocumentContainersWithObjectsAsync(entityId);
            //Add Root nodes
            if (!containers.IsNullOrEmpty())
            {
                foreach (var item in containers)
                {
                    ContainerTreeModel node = new ContainerTreeModel();
                    node.Label = item.Name;
                    node.Key = $"{item.ContainerId}";
                    node.Data = item.ContainerId.ToString();
                    node.Selectable = true;
                    node.ExpandedIcon = "pi pi-folder-open";
                    node.CollapsedIcon = "pi pi-folder";
                    node.Expanded = selectedContainer == item.ContainerId ? true : false;
                    containerTree.Add(node);
                }

                //Add child Nodes
                foreach (var item in containers)
                {
                    var folders = item.Documentobjects.Where(x => x.FileType == (int)DocumentObjectType.Folder).OrderBy(x => x.PathName).ToList();
                    foreach (var folder in folders)
                    {
                        var rootNode = GetContainerNode(containerTree, folder.ContainerId.ToString());
                        {
                            if (rootNode != null)
                            {
                                //check if folder is first level child
                                if (folder.PathName == rootNode.Label)
                                {
                                    ContainerTreeModel childNode = new ContainerTreeModel();
                                    childNode.Label = folder.FileName;
                                    childNode.Data = folder.PathName;
                                    childNode.Key = $"{folder.ContainerId}-{folder.DocumentObjectId}:{folder.PathName}/{folder.FileName}";
                                    childNode.Selectable = true;
                                    childNode.ExpandedIcon = "pi pi-folder-open";
                                    childNode.CollapsedIcon = "pi pi-folder";
                                    childNode.Expanded = selectedContainer == folder.ContainerId ? true : false;
                                    rootNode.Children.Add(childNode);
                                }
                                else
                                {
                                    var parentNode = GetParentNode(rootNode, folder);

                                    if (parentNode != null)
                                    {
                                        ContainerTreeModel childNode = new ContainerTreeModel();
                                        childNode.Label = folder.FileName;
                                        childNode.Data = folder.PathName;
                                        childNode.Key = $"{folder.ContainerId}-{folder.DocumentObjectId}:{folder.PathName}/{folder.FileName}";
                                        childNode.Selectable = true;
                                        childNode.ExpandedIcon = "pi pi-folder-open";
                                        childNode.CollapsedIcon = "pi pi-folder";
                                        childNode.Expanded = selectedFolder.StartsWith(folder.PathName) ? true : false;
                                        parentNode.Children.Add(childNode);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return containerTree;
        }
        public async Task<List<ContainerAccessModel>> GetContainerAccessListByContainerId(int id)
        {
            var accessList = new List<ContainerAccessModel>();

            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByIdAsync(id);

            if (container.AccessControlEnabled == (int)Status.Active)
            {
                foreach (var accessItem in container.Containeraccesses)
                {
                    var item = new ContainerAccessModel();

                    item.ContainerAccessId = accessItem.ContainerAccessId;
                    if (accessItem.MembershipTypeId != null)
                    {
                        item.MembershipCategoryId = accessItem.MembershipType.CategoryNavigation.MembershipCategoryId;
                        item.MembershipTypeId = accessItem.MembershipTypeId ?? 0;

                        var membershipType = new MembershipTypeModel();
                        membershipType.MembershipTypeId = accessItem.MembershipTypeId ?? 0;
                        membershipType.Name = accessItem.MembershipType.Name;
                        membershipType.CategoryName = accessItem.MembershipType.CategoryNavigation.Name;
                        item.MembershipType = membershipType;
                    }
                    else if (accessItem.GroupId != null)
                    {
                        item.GroupId = accessItem.GroupId ?? 0;
                        item.GroupName = accessItem.Group.GroupName;
                        item.GroupDescription = accessItem.Group.GroupDescription;
                        item.MembershipTypeId = 0;
                    }
                    if (accessItem.StaffRoleId != null)
                    {
                        item.StaffRoles = accessItem.StaffRoleId ?? 0;
                        item.GroupName = accessItem.StaffRole.Name;
                        item.GroupDescription = accessItem.StaffRole.Description;
                    }
                    accessList.Add(item);

                }
            }
            return accessList;
        }
        public async Task<List<DocumentObjectModel>> GetDocumentObjectsByContainerAndPath(int containerId, string filePath, int? entityId)
        {
            var documentObjects = new List<DocumentObjectModel>();

            var documents = await _unitOfWork.DocumentObjects.GetDocumentObjectsByContainerAndPathAsync(containerId, filePath);
            if(entityId!= null && entityId > 0)
            {
                documents = documents.Where(s => s.EntityId == entityId).ToList();
            }
            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByIdAsync(containerId);
            if (documents.Count() > 0)
            {
                foreach (var item in documents)
                {
                    var documentObject = new DocumentObjectModel();

                    documentObject.ContainerId = containerId;
                    documentObject.DocumentObjectId = item.DocumentObjectId;
                    documentObject.FileName = item.FileName;
                    documentObject.FileSize = item.FileSize ?? 0;
                    documentObject.DocumentTags = _mapper.Map<List<DocumentTagModel>>(item.Documenttags.ToList());
                    documentObject.CreatedDate = item.CreatedDate;
                    documentObject.CreatedBy = item.CreatedBy ?? 0;
                    //documentObject.CreatedByName = $"{item.CreatedByNavigation.FirstName} {item.CreatedByNavigation.FirstName}"; //comment for displaying first name two times
                    documentObject.CreatedByName = $"{item.CreatedByNavigation.FirstName}";
                    documentObject.BlobId = item.BlobId;
                    documentObject.AccessControlEnabled = container.AccessControlEnabled ?? 0;
                    var accessListArray = item.Documentaccesses.Select(x => x.GroupId.ToString()).ToArray();
                    documentObject.AccessList = string.Join(",", accessListArray);
                    documentObjects.Add(documentObject);
                }

            }
            documentObjects = documentObjects.OrderByDescending(x => x.CreatedDate).ToList();
            return documentObjects;
        }

        public async Task<List<DocumentObjectModel>> GetFoldersByContainerAndPath(int containerId, string filePath)
        {
            var documentObjects = new List<DocumentObjectModel>();

            var documents = await _unitOfWork.DocumentObjects.GetChildFolderssByContainerIdAndPathNameAsync(containerId, filePath);

            documentObjects = _mapper.Map<List<DocumentObjectModel>>(documents);

            return documentObjects;
        }

        public async Task<SolrSearchResultModel> GetDocumentObjectsByText(string text, string filter, string tags, DateTime fromdate, DateTime toDate, int entityId, int startPage, string sortBy, int staffUserId, string tenantId)
        {
            var documentObjects = new List<DocumentObjectModel>();

            _logger.Information($"GetDocumentObjectsByText: Tenant :{tenantId} Staff:{staffUserId} - Entity:{entityId} - Text:{text} - filter:{filter} - Tags:{tags} - Fromdate:{fromdate} - ToDate:{toDate} - Start page:{startPage} - Sort By:{sortBy}");

            if (filter == "undefined")
            {
                filter = "text";
            }
            bool validateAccess = false;
            var cacheKey = $"TenantId:{tenantId}-StaffUserId-{staffUserId}-EntityId:{entityId}-Text:{text}-Filter:{filter}";

            Stopwatch sw;

            string[] filterTags = tags.Split(",");

            var accessList = await _unitOfWork.Groups.GetAllGroupsByEntityIdAsync(entityId);

            if (accessList.ToList().Count > 0 & entityId > 0)
            {
                validateAccess = true;
            }

            var searchResultCount = _solrIndexService.GetSearchCountByText(text, filter, tenantId);
            _logger.Information($"GetDocumentObjectsByText Matched result count is {searchResultCount}");
            _logger.Information($"GetDocumentObjectsByText checking Cache for Key {cacheKey}");
            if (!_memoryCache.TryGetValue(cacheKey, out List<SolrDocumentModel> documents))
            {
                _logger.Information($"GetDocumentObjectsByText No Cache for Key {cacheKey}");
                sw = Stopwatch.StartNew();
                documents = (List<SolrDocumentModel>)_solrIndexService.GetDocumentsByText(text, filter, tenantId);
                _logger.Information($"GetDocumentObjectsByText: SolrService Call {cacheKey} Time:{sw.ElapsedMilliseconds} ms");
                sw.Stop();
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(600),
                    Priority = CacheItemPriority.High
                };
                //setting cache entries
                _logger.Information($"GetDocumentObjectsByText Setting Cache for Key {cacheKey}");
                _memoryCache.Set(cacheKey, documents, cacheExpiryOptions);
            }
            sw = Stopwatch.StartNew();
            foreach (var document in documents)
            {

                var model = await _unitOfWork.DocumentObjects.GetDocumentObjectByIdAsync(int.Parse(document.Id));
                if (model != null)
                {
                    if (model.Active == (int)Status.InActive)
                    {
                        continue;
                    }
                    if (filterTags.Length > 0 && tags.Length > 0)
                    {
                        if (!model.Documenttags.Any(x => filterTags.Contains(x.Tag.TagId.ToString())))
                        {
                            continue;
                        }
                    }
                    if (filter == "Date Range")
                    {
                        DateTime documentDate = (DateTime)model.CreatedDate;
                        if (documentDate.Date < fromdate.Date || documentDate.Date > toDate.Date)
                        {
                            continue;
                        }
                    }
                    //Check the container access
                    var containerAccess = await _unitOfWork.ContainerAccesses.GetContainerAccessByContainerIdAsync(model.ContainerId ?? 0);
                    if(entityId !=null || entityId !=0)
                    {
                        if (containerAccess != null)
                        {
                            foreach(var item in containerAccess)
                            {
                               if(item.Container.EntityId != entityId)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    var documentAccess = await _unitOfWork.DocumentObjectAccesses.GetDocumentAccessByDocumentObjectIdAsync(int.Parse(document.Id));
                    try
                    {
                        if (containerAccess.ToList().Count == 0 || entityId == 0)
                        {
                            if (staffUserId == 0)
                            {
                                var documentObject = _mapper.Map<DocumentObjectModel>(model);
                                documentObject.HighlightText = Regex.Replace(document.HighlightText, @"[\r\n]+", "", System.Text.RegularExpressions.RegexOptions.Multiline);
                                documentObject.Score = document.Score;
                                documentObjects.Add(documentObject);
                            }
                        }
                        else
                        {
                            if (validateAccess)
                            {
                                var documentAccessList = documentAccess.Select(x => x.GroupId).ToList();
                                var containerAccessList = containerAccess.Select(x => x.GroupId).ToList();

                                //If Documentlevel access is defined then first check that else Go for Container level access check
                                if (documentAccessList.Count() > 0)
                                {
                                    if (accessList.Any(x => documentAccessList.Contains(x.GroupId)))
                                    {
                                        //check if user has access to this document

                                        var documentObject = _mapper.Map<DocumentObjectModel>(model);
                                        documentObject.HighlightText = Regex.Replace(document.HighlightText, @"[\r\n]+", "", System.Text.RegularExpressions.RegexOptions.Multiline);
                                        documentObject.Score = document.Score;
                                        documentObjects.Add(documentObject);
                                    }
                                }
                                else
                                {
                                    if (accessList.Any(x => containerAccessList.Contains(x.GroupId)))
                                    {
                                        var documentObject = _mapper.Map<DocumentObjectModel>(model);
                                        documentObject.HighlightText = Regex.Replace(document.HighlightText, @"[\r\n]+", "", System.Text.RegularExpressions.RegexOptions.Multiline);
                                        documentObject.Score = document.Score;
                                        documentObjects.Add(documentObject);
                                    }
                                }
                            }
                        }

                        //Check if staff has a access

                        if (staffUserId != 0)
                        {
                            if (documentAccess.ToList().Count > 0 || containerAccess.ToList().Count > 0)
                            {
                                var roleAccess = await _unitOfWork.Staffusers.GetStaffUserByIdAsync(staffUserId);
                                try
                                {
                                    var documentAccessList = documentAccess.Select(x => x.StaffRoleId).ToList();
                                    var containerAccessList = containerAccess.Select(x => x.StaffRoleId).ToList();
                                    if (documentAccessList.Count() > 0)
                                    {
                                        if (roleAccess.Staffroles.Any(x => documentAccessList.ToList().Contains(x.RoleId)))
                                        {
                                            var documentObject = _mapper.Map<DocumentObjectModel>(model);
                                            documentObject.HighlightText = Regex.Replace(document.HighlightText, @"[\r\n]+", "", System.Text.RegularExpressions.RegexOptions.Multiline);
                                            documentObject.Score = document.Score;
                                            documentObjects.Add(documentObject);
                                        }
                                    }
                                    else
                                    {

                                        if (roleAccess.Staffroles.Any(x => containerAccessList.ToList().Contains(x.RoleId)))
                                        {
                                            var documentObject = _mapper.Map<DocumentObjectModel>(model);
                                            documentObject.HighlightText = Regex.Replace(document.HighlightText, @"[\r\n]+", "", System.Text.RegularExpressions.RegexOptions.Multiline);
                                            documentObject.Score = document.Score;
                                            if (!documentObjects.Any(x => x.DocumentObjectId == documentObject.DocumentObjectId))
                                                documentObjects.Add(documentObject);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error($"GetDocumentObjectsByText Error: {ex.Message}");
                                }
                            }
                            else
                            {
                                var documentObject = _mapper.Map<DocumentObjectModel>(model);
                                documentObject.HighlightText = Regex.Replace(document.HighlightText, @"[\r\n]+", "", System.Text.RegularExpressions.RegexOptions.Multiline);
                                documentObject.Score = document.Score;
                                if (!documentObjects.Any(x => x.DocumentObjectId == documentObject.DocumentObjectId))
                                    documentObjects.Add(documentObject);
                            }
                        }

                    }

                    catch (Exception ex)
                    {
                        _logger.Error($"GetDocumentObjectsByText Error:{ex.Message} {ex.StackTrace}");

                    }
                }
            }
            _logger.Information($"GetDocumentObjectsByText: Access Control Call {cacheKey} Time:{sw.ElapsedMilliseconds} ms");
            int startIndex = startPage * SOLR_PAGE_SIZE;
            int total = documentObjects.Count();
            SolrSearchResultModel results = new SolrSearchResultModel();
            results.StartPage = startIndex;
            results.TotalCount = total;
            results.TotalMatchCount = searchResultCount;
            if (total == 0)
            {
                results.DisplayMessage = $"No results found. Please refine your search terms to narrow your results.";
            }
            else if (total <= 100)
            {
                if (total <= 10)
                {
                    results.DisplayMessage = $"Displaying {total} best matches.";
                }
                else if (searchResultCount > 100)
                {
                    results.DisplayMessage = $"Displaying {startIndex + 1}-{startIndex + 10} best matches. Please refine your search to narrow your results.";
                }
                else
                {
                    results.DisplayMessage = $"Displaying {startIndex + 1}-{startIndex + 10} best matches.";
                }
            }



            if (sortBy.Length > 0)
            {
                if (sortBy.ToLower() == "relevance")
                {
                    documentObjects = documentObjects.OrderByDescending(x => x.Score).ToList();
                }
                else if (sortBy.ToLower() == "newest")
                {
                    documentObjects = documentObjects.OrderByDescending(x => x.CreatedDate).ToList();
                }
                else if (sortBy.ToLower() == "oldest")
                {
                    documentObjects = documentObjects.OrderBy(x => x.CreatedDate).ToList();
                }

            }
            if (total <= SOLR_PAGE_SIZE)
            {
                results.Documents = documentObjects;
            }
            else
            {
                results.Documents = documentObjects.Skip(startIndex).Take(SOLR_PAGE_SIZE).ToList();
            }

            return results;
        }

        public async Task<Boolean> DeleteFolder(DocumentObjectModel model)
        {
            if (model.DocumentObjectId == 0)
            {
                throw new InvalidOperationException("Root folder can't be deleted.");
            }

            var folder = await _unitOfWork.DocumentObjects.GetDocumentObjectByIdAsync(model.DocumentObjectId);

            if (folder != null)
            {
                //check if it has documents/folders
                var documents = await _unitOfWork.DocumentObjects.GetChildDocumentObjectsByContainerIdAndPathNameAsync(folder.ContainerId ?? 0, $"{folder.PathName}/{folder.FileName}");
                if (documents.IsNullOrEmpty())
                {
                    try
                    {
                        _unitOfWork.DocumentObjects.Remove(folder);
                        await _unitOfWork.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Could not delete folder :{folder.PathName}/{folder.FileName} Error:{ex.Message}");
                        throw new Exception($"Could not delete folder :{folder.PathName}/{folder.FileName}.");
                    }

                }
                else
                {
                    throw new InvalidOperationException("This folder can't be delete as there are sub folders  or documents under it.");
                }
            }

            return false;
        }

        public async Task<DocumentObjectModel> UpdateFolder(DocumentObjectModel model)
        {

            var documentObject = await _unitOfWork.DocumentObjects.GetDocumentObjectByIdAsync(model.DocumentObjectId);

            string oldPathName = Path.Combine(documentObject.PathName, documentObject.FileName);
            string newPathName = Path.Combine(documentObject.PathName, model.FileName);

            if (documentObject != null)
            {
                //validate if a folder already exists with same name

                var existingObjects = await _unitOfWork.DocumentObjects.GetChildFolderssByContainerIdAndPathNameAsync(model.ContainerId, documentObject.PathName);
                if (existingObjects.Any(x => x.FileName == model.FileName))
                {
                    _logger.Error($"A folder already exists with same name {newPathName}");
                    throw new Exception($"A folder already exists with same name {newPathName}");
                }
                var childObjects = await _unitOfWork.DocumentObjects.GetChildDocumentObjectsByContainerIdAndPathNameAsync(model.ContainerId, Path.Combine(documentObject.PathName, documentObject.FileName));

                //update FileName

                documentObject.FileName = model.FileName;
                try
                {
                    _unitOfWork.DocumentObjects.Update(documentObject);

                    //If there are child objects then change the foldername

                    foreach (var childObject in childObjects)
                    {
                        childObject.PathName = childObject.PathName.ReplaceFirst(oldPathName, newPathName);
                        _unitOfWork.DocumentObjects.Update(childObject);
                    }
                    await _unitOfWork.CommitAsync();

                }
                catch (Exception ex)
                {
                    _logger.Error($"Could not update folder :{model.PathName}/{model.FileName} Error:{ex.Message}");
                    throw new Exception($"Could not update folder :{model.PathName}/{model.FileName}.");
                }


            }

            return _mapper.Map<DocumentObjectModel>(documentObject);

        }

        public async Task<Boolean> DeleteDocument(DocumentObjectRequestModel model)
        {
            var document = await _unitOfWork.DocumentObjects.GetDocumentObjectByIdAsync(model.DocumentObjectId);
            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByIdAsync(document.ContainerId ?? 0);

            if (document != null)
            {
                try
                {
                    //Removed document from Solr
                    SolrDocumentModel solrDocument = new SolrDocumentModel();

                    solrDocument.Id = model.DocumentObjectId.ToString();

                    _solrIndexService.Delete(solrDocument);

                    //Remove Document from Azure

                    var result = await _azureStorageService.DeleteBlob(model.OrganizationId, container.BlobContainerId, document.BlobId);

                    //Check if it has access control

                    var accessList = await _unitOfWork.DocumentObjectAccesses.GetDocumentAccessByDocumentObjectIdAsync(document.DocumentObjectId);
                    if (accessList.Count() > 0)
                    {
                        _unitOfWork.DocumentObjectAccesses.RemoveRange(accessList);
                    }


                    //Mark the document Inactive
                    document.Active = (int)Status.InActive;
                    _unitOfWork.DocumentObjects.Update(document);
                    await _unitOfWork.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Could not delete document :{document.PathName}/{document.FileName} Error:{ex.Message}");
                    throw new Exception($"Could not delete document :{document.PathName}/{document.FileName}.");
                }

            }

            return false;
        }

        public async Task<List<DocumentAccessModel>> GetDocumentAccessListByGroupId(int groupId)
        {
            List<DocumentAccessModel> accessList = new List<DocumentAccessModel>();
            var documentAccessList = await _unitOfWork.DocumentObjectAccesses.GetDocumentObjectAccessListByGroupIdAsync(groupId);

            if (documentAccessList != null)
            {
                accessList = _mapper.Map<List<DocumentAccessModel>>(documentAccessList);
            }
            return accessList;
        }

        public async Task<List<ContainerAccessModel>> GetContainerAccessListByGroupId(int groupId)
        {
            List<ContainerAccessModel> accessList = new List<ContainerAccessModel>();
            var containerAccessList = await _unitOfWork.ContainerAccesses.GetContainerAccessByGroupIdAsync(groupId);

            if (containerAccessList != null)
            {
                accessList = _mapper.Map<List<ContainerAccessModel>>(containerAccessList);
            }
            return accessList;
        }

        public async Task<DocumentObjectModel> UpdateDocumentAccessControl(DocumentObjectModel model)
        {
            //Check if Access at document level is defined

            var container = await _unitOfWork.DocumentContainers.GetDocumentContainerByIdAsync(model.ContainerId);

            if (container.AccessControlEnabled == (int)Status.Active)
            {
                var accessGroups = model.AccessList.Split(",");
                if (accessGroups.Length > 0)
                {
                    var containerAccess = await _unitOfWork.ContainerAccesses.GetContainerAccessByContainerIdAsync(container.ContainerId);

                    //First remove old selection
                    var documentAccesses = await _unitOfWork.DocumentObjectAccesses.GetDocumentAccessByDocumentObjectIdAsync(model.DocumentObjectId);
                    if (documentAccesses.Count() > 0)
                    {
                        _unitOfWork.DocumentObjectAccesses.RemoveRange(documentAccesses);
                        await _unitOfWork.CommitAsync();
                    }

                    if (accessGroups.Length != containerAccess.Count())
                    {
                        foreach (var accessItem in containerAccess)
                        {
                            if (accessGroups.Contains(accessItem.GroupId.ToString()))
                            {
                                var documentAccess = new Documentaccess();
                                documentAccess.DocumentObjectId = model.DocumentObjectId;
                                documentAccess.GroupId = accessItem.GroupId;
                                await _unitOfWork.DocumentObjectAccesses.AddAsync(documentAccess);
                            }
                        }
                        foreach (var staffItem in containerAccess)
                        {
                            if (model.StaffRoles.Contains(staffItem.StaffRoleId ?? 0))
                            {
                                var documentAccess = new Documentaccess();
                                documentAccess.DocumentObjectId = model.DocumentObjectId;
                                documentAccess.StaffRoleId = staffItem.StaffRoleId;
                                await _unitOfWork.DocumentObjectAccesses.AddAsync(documentAccess);
                            }
                        }
                        await _unitOfWork.CommitAsync();
                    }
                }
            }

            var document = await _unitOfWork.DocumentObjects.GetDocumentObjectByIdAsync(model.DocumentObjectId);

            return _mapper.Map<DocumentObjectModel>(document);
        }

        public async Task<DocumentObjectModel> UpdateDocumentTags(DocumentObjectModel model)
        {
            //Remove Old Tags
            var documentTags = await _unitOfWork.DocumentObjectTags.GetDocumentTagsByDocumentObjectIdAsync(model.DocumentObjectId);
            if (documentTags.Count() > 0)
            {
                _unitOfWork.DocumentObjectTags.RemoveRange(documentTags);
                await _unitOfWork.CommitAsync();
            }

            //Check if Tags are defined

            if (model.TagList.Length > 0)
            {
                var tags = model.TagList.Split(",");

                foreach (var tag in tags)
                {
                    if (tag.Length > 0)
                    {
                        Documenttag documentTag = new Documenttag();
                        documentTag.TagId = Int32.Parse(tag);
                        documentTag.DocumentObjectId = model.DocumentObjectId;
                        await _unitOfWork.DocumentObjectTags.AddAsync(documentTag);
                    }
                }
                await _unitOfWork.CommitAsync();
            }

            var document = await _unitOfWork.DocumentObjects.GetDocumentObjectByIdAsync(model.DocumentObjectId);

            return _mapper.Map<DocumentObjectModel>(document);
        }

        public async Task<DocumentTagModel> AddTagToDocumentObject(DocumentTagModel model)
        {
            var tags = await _unitOfWork.DocumentObjectTags.GetDocumentTagsByTagIdAsync(model.TagId ?? 0);
            if (tags != null)
            {
                Documenttag tag = new Documenttag();
                tag.DocumentObjectId = model.DocumentObjectId;
                tag.TagId = model.TagId;
                tag.TagValue = model.TagValue;
                try
                {
                    await _unitOfWork.DocumentObjectTags.AddAsync(tag);
                    await _unitOfWork.CommitAsync();
                    return _mapper.Map<DocumentTagModel>(tag);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to add Tag.");
                }
            }
            else
            {
                throw new Exception("Document alreday has a tag.");
            }
        }
        public async Task<Boolean> RemoveTagFromDocumentObject(DocumentTagModel model)
        {
            var tags = await _unitOfWork.DocumentObjectTags.GetDocumentTagsByDocumentObjectIdAsync(model.DocumentObjectId ?? 0);
            if (tags != null)
            {
                Documenttag tag = tags.Where(x => x.TagId == model.TagId).FirstOrDefault();
                try
                {
                    _unitOfWork.DocumentObjectTags.Remove(tag);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to remove Tag.");
                }
            }
            else
            {
                throw new Exception("Document does not have the requested tag.");
            }
        }
        public async Task<List<DocumentTagModel>> GetTagsByDocumentObjectId(int id)
        {
            var tags = await _unitOfWork.DocumentObjectTags.GetDocumentTagsByDocumentObjectIdAsync(id);
            return _mapper.Map<List<DocumentTagModel>>(tags);
        }

        public async Task<List<DocumentTagModel>> GetDocumentObjectsByTagId(int id)
        {
            var tags = await _unitOfWork.DocumentObjectTags.GetDocumentObjectsByTagIdAsync(id);
            return _mapper.Map<List<DocumentTagModel>>(tags);
        }

        public async Task<List<DocumentObjectAccessHistoryModel>> GetAuditTrailByDocumentId(int id)
        {
            var accessList = new List<DocumentObjectAccessHistoryModel>();
            if (id > 0)
            {
                var auditTrail = await _unitOfWork.DocumentObjectAccessHistories.GetDocumentAccessHistoryByDocumentObjectIdAsync(id);

                if (auditTrail != null)
                {
                    foreach (var item in auditTrail)
                    {
                        var accessItem = new DocumentObjectAccessHistoryModel();
                        accessItem.AccessId = item.AccessId;
                        accessItem.AccessDate = (DateTime)item.AccessDate;
                        accessItem.AccessType = item.AccessType ?? 0;
                        accessItem.DocumentName = item.DocumentObject.FileName;
                        //accessItem.MemberName = item.Entity.Name;
                        //accessItem.EntityId = item.EntityId ?? 0;
                        if (item.Entity != null)
                        {
                            accessItem.MemberName = item.Entity.Name;
                            accessItem.EntityId = item.EntityId ?? 0;
                        }
                        else
                        {
                            accessItem.MemberName = item.StaffUser.FirstName + " " + item.StaffUser.LastName;
                            accessItem.StaffUserId = item.StaffUserId ?? 0;//item.EntityId ?? 0;
                        }
                        if (item.EntityId > 0)
                        {
                            accessItem.UserType = item.Entity.People?.FirstOrDefault(x => x.EntityId == item.EntityId)?.Designation;
                        }
                        if (item.StaffUserId > 0)
                        {
                            accessItem.UserType = "Staff User";
                        }
                        accessList.Add(accessItem);

                    }
                    accessList = accessList.OrderByDescending(x => x.AccessDate).ToList();
                    return accessList;
                }
            }
            else
            {
                throw new Exception("Invalid Document Id.");
            }
            return new List<DocumentObjectAccessHistoryModel>();
        }
        public async Task<List<DocumentObjectAccessHistoryModel>> GetAuditTrailByDateRange(DateTime startDate, DateTime endDate)
        {
            var accessList = new List<DocumentObjectAccessHistoryModel>();

            var auditTrail = await _unitOfWork.DocumentObjectAccessHistories.GetAllDocumentAccessHistoryAsync();

            if (auditTrail != null)
            {
                foreach (var item in auditTrail)
                {
                    var accessItem = new DocumentObjectAccessHistoryModel();
                    accessItem.AccessId = item.AccessId;
                    accessItem.AccessDate = (DateTime)item.AccessDate;
                    accessItem.AccessType = item.AccessType ?? 0;
                    accessItem.DocumentName = item.DocumentObject.FileName;
                    accessItem.MemberName = item.Entity.Name;
                    accessItem.EntityId = item.EntityId ?? 0;
                    accessList.Add(accessItem);

                }
                return accessList;
            }
            else
            {
                return new List<DocumentObjectAccessHistoryModel>();
            }

        }

        public async Task<List<DocumentAccessModel>> GetDocumentAccessListByDocumentId(int id)
        {
            var accessList = new List<DocumentAccessModel>();

            var documentAccessList = await _unitOfWork.DocumentObjectAccesses.GetDocumentAccessByDocumentObjectIdAsync(id);

            accessList = _mapper.Map<List<DocumentAccessModel>>(documentAccessList);

            return accessList;
        }
        public async Task<List<DocumentTagModel>> GetDocumentTagListById(int id)
        {
            var tagList = new List<DocumentTagModel>();

            var documentTagList = await _unitOfWork.DocumentObjectTags.GetDocumentTagsByDocumentObjectIdAsync(id);

            tagList = _mapper.Map<List<DocumentTagModel>>(documentTagList);

            return tagList;
        }
        public async Task<BarChartModel> GetSearchStatistics()
        {
            var graphData = await _unitOfWork.DocumentObjectAccessHistories.GetDocumentAccessHistoryByDocument();
            var barChartModel = new BarChartModel();
            BarChartDataset dataSet = new BarChartDataset();

            foreach (var item in graphData)
            {
                barChartModel.Labels.Add(item.GroupName);
                dataSet.Data.Add(item.Value);
                dataSet.BackgroundColor.Add(GraphHelper.GetRandomColor());
                barChartModel.Datasets.Add(dataSet);
            }

            return barChartModel;
        }

        public async Task<BarChartModel> GetMemberPortalActiveUsers()
        {
            var barChartModel = new BarChartModel();

            var topLogins = await _unitOfWork.AccessLogs.GetTop10AccessLogAsync();

            BarChartDataset dataSet = new BarChartDataset();

            foreach (var user in topLogins)
            {
                barChartModel.Labels.Add(user.UserName);
                dataSet.Data.Add(user.Count);
                dataSet.BackgroundColor.Add(GraphHelper.GetRandomColor());
                barChartModel.Datasets.Add(dataSet);
            }

            return barChartModel;
        }

        public async Task<bool> ExportDocuments(string userName)
        {
            var exported = false;
            int startPage = 0;
            Boolean exportDone = false;

            while (!exportDone)
            {
                try
                {
                    var results = _solrIndexService.ExportDocuments(startPage);
                    if (results.Count() > 0)
                    {
                        //Update Database
                        foreach (var document in results)
                        {
                            var solrExport = new Solrexport();
                            solrExport.SolrId = int.Parse(document.Id);
                            solrExport.FileName = document.FileName;
                            solrExport.Text = document.Text;
                            solrExport.CreatedBy = document.CreatedBy;
                            solrExport.CreatedDate = DateTime.Parse(document.CreatedDate.ToString());
                            await _unitOfWork.SolrExports.AddAsync(solrExport);
                            await _unitOfWork.CommitAsync();
                        }
                        startPage++;

                    }
                    else
                    {
                        exportDone = true;
                        exported = true;
                        return exported;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"ExportDocuments Error:{ex.Message} {ex.StackTrace}");
                    throw new Exception("Unable to Export Documents.");
                }
            }
            return exported;
        }

        public bool UploadSolrDocument(SolrDocumentModel model)
        {
            SolrDocumentModel solrDocument = new SolrDocumentModel();
            solrDocument.Id = model.Id;
            solrDocument.TenantId = model.TenantId;
            solrDocument.Text = model.Text;
            solrDocument.CreatedBy = model.CreatedBy;
            solrDocument.CreatedDate = (DateTime)model.CreatedDate;
            solrDocument.FileName = model.FileName;
            try
            {
                _solrIndexService.AddUpdate(solrDocument);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"UpploadSolrDocument Error:{ex.Message} {ex.StackTrace}");
                throw new Exception("Unable to Upload Solr Document.");
            }
        }

        private string GetObjectId()
        {
            return (Guid.NewGuid().ToString());
        }


        private string GetEncryptionKey()
        {
            int length = 24;

            // creating a StringBuilder object()
            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                sb.Append(letter);
            }
            return sb.ToString();
        }
        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }

        private void AddChildNode(ContainerTreeModel parent, Documentobject folder)
        {

            ContainerTreeModel childNode = new ContainerTreeModel();
            childNode.Label = folder.FileName;
            childNode.Data = folder.PathName;
            childNode.Selectable = true;
            childNode.ExpandedIcon = "pi pi-folder-open";
            childNode.CollapsedIcon = "pi pi-folder";
            parent.Children.Add(childNode);

        }

        private ContainerTreeModel GetParentNode(ContainerTreeModel rootNode, Documentobject folder)
        {
            if (rootNode.Children.Count > 0)
            {
                foreach (var node in rootNode.Children)
                {
                    if (node.Data + '/' + node.Label == folder.PathName)
                    {
                        return node;
                    }
                    else if (node.Children.Count > 0)
                    {
                        var childNode = GetParentNode(node, folder);
                        if (childNode != null)
                        {
                            return childNode;
                        }
                    }
                }
            }
            return null;
        }
        private ContainerTreeModel GetContainerNode(List<ContainerTreeModel> tree, string containerId)
        {
            if (tree.Count > 0)
            {
                foreach (var node in tree)
                {
                    if (node.Data == containerId)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        private DateTime CreateDateTime(string date)
        {
            string dateStr = date.Remove(0, 2); //Remove D:
            string tmpDateStr = dateStr.Substring(0, 4) //Get year i.e yyyy
                + "-" + dateStr.Substring(4, 2) // Get month i.e mm & prepend - (hyphen)
                + "-" + dateStr.Substring(6, 2) // Get day i.e dd & prepend -
                + "T" + dateStr.Substring(8, 2) // Get hour and prepend T
                + ":" + dateStr.Substring(10, 2) // Get minutes and prepend :
                + ":" + dateStr.Substring(12, 2); //Get seconds and prepend :

            try
            {
                return DateTime.Parse(tmpDateStr);
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }

        }

    }
}