using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Xml.Linq;
using System.Runtime.InteropServices.ComTypes;
using Max.Services.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Twilio.TwiML.Voice;
using System.Text;

namespace Max.Services
{
    public class DocumentService : IDocumentService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostEnvironment _appEnvironment;
        private readonly ILogger<DocumentService> _logger;
        private readonly ISociableService _sociableService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DocumentService(IUnitOfWork unitOfWork,
            IHostEnvironment appEnvironment, ILogger<DocumentService> logger, ISociableService sociableService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _appEnvironment = appEnvironment;
            _logger = logger;
            _sociableService = sociableService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Document>> GetAllDocuments()
        {
            return await _unitOfWork.Documents
                .GetAllDocumentsAsync();
        }

        public async Task<Document> GetDocumentById(int id)
        {
            return await _unitOfWork.Documents
                .GetDocumentByIdAsync(id);
        }
        public async Task<IEnumerable<Document>> GetDocumentsByEntityId(int id)
        {
            return await _unitOfWork.Documents
                .GetDocumentsByEntityIdAsync(id);
        }

        public async Task<Document> GetPictureByEntityId(int id)
        {
            return await _unitOfWork.Documents
                .GetPictureByEntityIdAsync(id);
        }

        public async Task<Document> CreateDocument(IFormFile file, DocumentModel model)
        {
            Entity entity = new Entity();
            Staffuser staff = new Staffuser();
            Document document = new Document();
            var isValid = ValidDocument(model);
            if (isValid)
            {

                string root = model.DocumentRoot;
                string personFolderPath = "";
                string organizationFolderPath = "";
                string staffFolderPath = "";
                string mappedPath = "";
                string eventFolderPath = "";

                if (model.EntityId > 0)
                {
                    personFolderPath = $"\\Documents\\{model.TenantId}\\EntityId-{model.EntityId}";
                    mappedPath = Path.GetFullPath(root + personFolderPath);
                }
                if (model.OrganizationId > 0)
                {
                    organizationFolderPath = $"\\Documents\\{model.TenantId}\\Organization\\{model.OrganizationId}";
                    mappedPath = Path.GetFullPath(root + organizationFolderPath);
                }

                if (model.StaffId > 0)
                {
                    staffFolderPath = $"\\Documents\\{model.TenantId}\\Staff\\StaffId-{model.StaffId}";
                    mappedPath = Path.GetFullPath(root + staffFolderPath);
                }

                if (model.EventId > 0)
                {

                    eventFolderPath = $"\\Documents\\{model.TenantId}\\Event\\EventId-{model.EventId}";
                    mappedPath = Path.GetFullPath(root + eventFolderPath);

                    if (model.EventBannerImageId > 0)
                    {
                        eventFolderPath = $"\\Documents\\{model.TenantId}\\Event\\EventId-{model.EventId}\\EventBannerImage-{model.EventId}";
                        mappedPath = Path.GetFullPath(root + eventFolderPath);
                    }
                }

                // If directory does not exist, create it. 
                if (!Directory.Exists(mappedPath))
                {
                    Directory.CreateDirectory(mappedPath);
                }

                string extention = Path.GetExtension(file.FileName);
                string fileName = Guid.NewGuid().ToString().Replace("-", "") + extention;

                if (!ValidateExtention(file))
                {
                    return document;
                }

                try
                {
                    string absolutePath = Path.GetFullPath(Path.Combine(mappedPath, fileName));
                    using (var stream = new FileStream(absolutePath, FileMode.Create))
                    {
                        if (model.EntityId > 0)
                        {
                            document.EntityId = model.EntityId;
                            document.FilePath = personFolderPath;
                        }

                        if (model.OrganizationId > 0)
                        {
                            document.OrganizationId = model.OrganizationId;
                            document.FilePath = organizationFolderPath;
                        }

                        if (model.StaffId > 0)
                        {
                            document.StaffId = model.StaffId;
                            document.FilePath = staffFolderPath;
                        }

                        if (model.EventId > 0 || model.EventBannerImageId > 0)
                        {
                            document.EventId = model.EventId;
                            document.FilePath = eventFolderPath;
                        }

                        document.OrganizationId = model.OrganizationId;
                        document.FileName = fileName;
                        document.DisplayFileName = file.GetFileName();
                        document.ContentType = GetContentType(extention);
                        document.Title = model.Title;

                        if (model.EntityId > 0)
                        {
                            var picture = await _unitOfWork.Documents.GetPictureByEntityIdAsync(model.EntityId);
                            if (picture != null)
                            {
                                string fileToDeleteabsolutePath = Path.GetFullPath(Path.Combine(mappedPath, picture.FileName));
                                if (File.Exists(fileToDeleteabsolutePath))
                                {
                                    // If file found, delete it    
                                    File.Delete(fileToDeleteabsolutePath);
                                }
                                _unitOfWork.Documents.Remove(picture);
                                await _unitOfWork.Documents.AddAsync(document);
                            }
                            else
                            {
                                await _unitOfWork.Documents.AddAsync(document);

                            }
                            await _unitOfWork.CommitAsync();
                            file.CopyTo(stream);


                            entity = await _unitOfWork.Entities.GetByIdAsync(model.EntityId);
                            entity.ProfilePictureId = document.DocumentId;
                            _unitOfWork.Entities.Update(entity);
                            await _unitOfWork.CommitAsync();

                        }
                        if (model.StaffId > 0)
                        {
                            var picture = await _unitOfWork.Documents.GetProfilePictureByStaffIdAsync(model.StaffId);
                            if (picture != null)
                            {
                                string fileToDeleteabsolutePath = Path.GetFullPath(Path.Combine(mappedPath, picture.FileName));
                                if (File.Exists(fileToDeleteabsolutePath))
                                {
                                    // If file found, delete it    
                                    File.Delete(fileToDeleteabsolutePath);
                                }
                                _unitOfWork.Documents.Remove(picture);
                                await _unitOfWork.Documents.AddAsync(document);
                            }
                            else
                            {
                                await _unitOfWork.Documents.AddAsync(document);

                            }
                            await _unitOfWork.CommitAsync();
                            file.CopyTo(stream);


                            staff = await _unitOfWork.Staffusers.GetByIdAsync(model.StaffId);
                            staff.ProfilePictureId = document.DocumentId;
                            _unitOfWork.Staffusers.Update(staff);
                            await _unitOfWork.CommitAsync();

                        }
                        if (model.OrganizationId > 0)
                        {
                            var picture = await _unitOfWork.Documents.GetImageByOrganizationIdAndTitleAsync(model.OrganizationId ?? 0, model.Title);

                            if (picture != null)
                            {
                                _unitOfWork.Documents.Remove(picture);
                                await _unitOfWork.Documents.AddAsync(document);
                            }
                            else
                            {
                                await _unitOfWork.Documents.AddAsync(document);
                            }
                            await _unitOfWork.CommitAsync();
                            file.CopyTo(stream);


                            var organization = await _unitOfWork.Organizations.GetOrganizationByIdAsync(model.OrganizationId ?? 0);
                            if (model.Title == "header")
                            {
                                organization.HeaderImage = document.FilePath + "\\" + document.FileName;
                            }
                            if (model.Title == "footer")
                            {
                                organization.FooterImge = document.FilePath + "\\" + document.FileName;
                            }
                            if (model.Title == "logo")
                            {
                                organization.Logo = document.FilePath + "\\" + document.FileName;
                            }
                            _unitOfWork.Organizations.Update(organization);
                            await _unitOfWork.CommitAsync();
                        }
                        if (model.EventId > 0 && model.EventBannerImageId == 0)
                        {
                            var picture = await _unitOfWork.Documents.GetPictureByEventIdAsync(model.EventId);
                            if (picture != null)
                            {
                                string fileToDeleteabsolutePath = Path.GetFullPath(Path.Combine(mappedPath, picture.FileName));
                                if (File.Exists(fileToDeleteabsolutePath))
                                {
                                    // If file found, delete it    
                                    File.Delete(fileToDeleteabsolutePath);
                                }
                                //_unitOfWork.Documents.Remove(picture);
                                await _unitOfWork.Documents.AddAsync(document);
                            }
                            else
                            {
                                await _unitOfWork.Documents.AddAsync(document);

                            }
                            await _unitOfWork.CommitAsync();
                            file.CopyTo(stream);


                            var events = await _unitOfWork.Events.GetByIdAsync(model.EventId);
                            events.EventImageId = document.DocumentId;
                            _unitOfWork.Events.Update(events);
                            await _unitOfWork.CommitAsync();
                        }


                        if (model.EventBannerImageId > 0)
                        {
                            var picture = await _unitOfWork.Documents.GetBannerByEventIdAsync(model.EventBannerImageId);
                            if (picture != null)
                            {
                                string fileToDeleteabsolutePath = Path.GetFullPath(Path.Combine(mappedPath, picture.FileName));
                                if (File.Exists(fileToDeleteabsolutePath))
                                {
                                    // If file found, delete it    
                                    File.Delete(fileToDeleteabsolutePath);
                                }
                                //_unitOfWork.Documents.Remove(picture);
                                await _unitOfWork.Documents.AddAsync(document);
                            }
                            else
                            {
                                await _unitOfWork.Documents.AddAsync(document);

                            }
                            await _unitOfWork.CommitAsync();
                            file.CopyTo(stream);

                            var events = await _unitOfWork.Events.GetByIdAsync(model.EventId);
                            events.EventBannerImageId = document.DocumentId;
                            _unitOfWork.Events.Update(events);
                            await _unitOfWork.CommitAsync();
                        }
                    }

                    //Update member / staff profile picture to sociable Profile
                    if (model.EntityId > 0 || model.StaffId > 0)
                    {
                        var staffUser = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                        int organizationId = entity.OrganizationId != null ? entity.OrganizationId ?? 0 : staff.OrganizationId;
                        var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(organizationId);
                        if (configuration.SociableSyncEnabled == (int)Status.Active && staff != null )
                        {
                            if (model.EntityId > 0)
                            {
                                try
                                {
                                    var sociableDocument = await GetProfileImageById(root, model.TenantId, model.EntityId);
                                    model.Document = sociableDocument.Document;
                                    model.OrganizationId = entity.OrganizationId;
                                    model.FileName = sociableDocument.FileName;
                                    model.DisplayFileName = sociableDocument.DisplayFileName;
                                    model.ContentType = sociableDocument.ContentType;
                                    await _sociableService.UpdateProfileImage(model);

                                }

                                catch (Exception ex)
                                {
                                    _logger.LogError($"Update Person Profile Picture: Failed to update profile picture: {entity.Name} {entity.WebLoginName} failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                                }
                            }

                            else if (model.StaffId > 0)
                            {
                                try
                                {
                                    var sociableDocument = await GetStaffProfileImageById(root, model.TenantId, model.StaffId);
                                    model.Document = sociableDocument.Document;
                                    model.OrganizationId = staff.OrganizationId;
                                    model.FileName = sociableDocument.FileName;
                                    model.DisplayFileName = sociableDocument.DisplayFileName;
                                    model.ContentType = sociableDocument.ContentType;
                                    await _sociableService.UpdateProfileImage(model);

                                }

                                catch (Exception ex)
                                {
                                    _logger.LogError($"Update staff Profile Picture: Failed to update profile picture: {staff.FirstName} {staff.LastName} failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                                }

                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return document;
        }

        public async Task<DocumentModel> GetProfileImageById(string documentRoot, string tenantId, int entityId)
        {
            DocumentModel model = new DocumentModel();

            var memory = new MemoryStream();

            if (entityId > 0)
            {
                var entity = await _unitOfWork.Entities.GetEntityByIdAsync(entityId);
                var imageId = entity.ProfilePictureId ?? 0;
                var document = await _unitOfWork.Documents.GetDocumentByIdAsync(imageId);
                if (document != null)
                {
                  
                    var filePath = documentRoot + document.FilePath + "\\" + document.FileName;
                    model.ContentType = document.ContentType;
                    model.FileName = document.FileName;
                    model.DisplayFileName = document.DisplayFileName;
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    model.Document = memory.ToArray();
                   
                }
                else
                {
                    //Read Default
                    if (entity.PersonId > 0)
                    {
                        var fileName = "b5fd8dcbc82349fe9e59637e18790367.jpg"; //TODO: AKS This needs to come from config
                        var filePath = documentRoot + "\\Documents\\EntityId-0\\" + fileName; //TODO: AKS This needs to come from config
                        model.ContentType = "image/jpeg";
                        model.FileName = "profile.jpg";
                        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await stream.CopyToAsync(memory);
                        }
                        memory.Position = 0;
                        model.Document = memory.ToArray();
                    }
                    else if(entity.CompanyId > 0)
                    {
                        var fileName = "NoProfile.png"; //TODO: AKS This needs to come from config
                        var filePath = documentRoot + "\\Documents\\CompanyId-0\\" + fileName; //TODO: AKS This needs to come from config
                        model.ContentType = "image/jpeg";
                        model.FileName = "profile.jpg";
                        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await stream.CopyToAsync(memory);
                        }
                        memory.Position = 0;
                        model.Document = memory.ToArray();
                    }
                }
            }
            else
            {
                //Read Default
                var fileName = "b5fd8dcbc82349fe9e59637e18790367.jpg"; //TODO: AKS This needs to come from config
                var filePath = documentRoot + "\\Documents\\EntityId-0\\" + fileName; //TODO: AKS This needs to come from config
                model.ContentType = "image/jpeg";
                model.FileName = "profile.jpg";
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                model.Document = memory.ToArray();
            }
            return model;
        }

        public async Task<DocumentModel> GetOrganizationImageByOrgIdAndTitle(string documentRoot, int organizationId, string title)
        {
            DocumentModel model = new DocumentModel();
            Organization organization = new Organization();

            var memory = new MemoryStream();
            var imagePath = "";

            if (organizationId > 0)
            {
                try
                {
                    organization = await _unitOfWork.Organizations.GetOrganizationByIdAsync(organizationId);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Cant find organization");
                }

                if (title == "header")
                {
                    imagePath = organization.HeaderImage;
                    if (imagePath == null)
                    {
                        imagePath = "\\Documents\\Organization\\empty-image.png";
                    }
                }
                if (title == "footer")
                {
                    imagePath = organization.FooterImge;
                    if (imagePath == null)
                    {
                        imagePath = "\\Documents\\Organization\\empty-image.png";
                    }
                }
                if (title == "logo")
                {
                    imagePath = organization.Logo;
                    if (imagePath == null)
                    {
                        imagePath = "\\Documents\\Organization\\empty-image.png";
                    }
                }

                var filePath = documentRoot + imagePath;
                if (File.Exists(filePath))
                {
                    model.ContentType = GetContentType(filePath);

                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }

                    memory.Position = 0;
                    model.Document = memory.ToArray();
                }
                else
                {
                    model.ContentType = "text/plain";
                    var okMessageBytes = Encoding.UTF8.GetBytes("Image not found");
                    model.Document = okMessageBytes;
                }
            }
            return model;
        }
        public async Task<bool> DeleteProfileImageByEntityId(int entityId, string rootFolder)
        {
            var entity = await _unitOfWork.Entities.GetByIdAsync(entityId);


            if (entity != null)
            {
                int imageId = entity.ProfilePictureId ?? 0;

                if (imageId > 0)
                {
                    var picture = await _unitOfWork.Documents.GetPictureByEntityIdAsync(entityId);
                    if (picture != null)
                    {
                        string personFolderPath = "\\Documents\\EntityId-" + entityId;
                        string mappedPath = Path.GetFullPath(rootFolder + personFolderPath);
                        string fileToDeleteabsolutePath = Path.GetFullPath(Path.Combine(mappedPath, picture.FileName));
                        if (File.Exists(fileToDeleteabsolutePath))
                        {
                            // If file found, delete it    
                            File.Delete(fileToDeleteabsolutePath);
                        }
                        _unitOfWork.Documents.Remove(picture);
                        entity.ProfilePictureId = 0;
                        _unitOfWork.Entities.Update(entity);
                        await _unitOfWork.CommitAsync();

                        //Update member / staff profile picture to sociable Profile
                        int organizationId = entity.OrganizationId ?? 0;
                        var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(organizationId);
                        var staffUser = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                        if (configuration.SociableSyncEnabled == (int)Status.Active && staffUser != null)
                        {
                            try
                            {
                                DocumentModel document = new DocumentModel();
                                document.EntityId = entityId;
                                var memory = new MemoryStream();

                                var fileName = "b5fd8dcbc82349fe9e59637e18790367.jpg"; //TODO: AKS This needs to come from config
                                var filePath = rootFolder + "\\Documents\\EntityId-0\\" + fileName; //TODO: AKS This needs to come from config
                                document.ContentType = "image/jpeg";
                                document.DisplayFileName = "default_profile.jpg";
                                document.OrganizationId = organizationId;

                                using (var stream = new FileStream(filePath, FileMode.Open))
                                {
                                    await stream.CopyToAsync(memory);
                                }
                                memory.Position = 0;
                                document.Document = memory.ToArray();
                                return await _sociableService.UpdateProfileImage(document);

                            }

                            catch (Exception ex)
                            {
                                _logger.LogError($"Update Person Profile Picture: Failed to update profile picture: {entity.Name} {entity.WebLoginName} failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                            }
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ValidDocument(DocumentModel model)
        {
            //Validate  Name
            /*
             if (model.Title.IsNullOrEmpty())
             {
                 throw new InvalidOperationException($"Document Title can not be empty.");
             }

             if (model.FileName.IsNullOrEmpty())
             {
                 throw new InvalidOperationException($"Document FileName can not be empty.");
             }
            */
            return true;
        }

        private async Task<string> GetPath(int personId, int documentId)
        {
            return await _unitOfWork.Documents.GetDocumentPathAsync(personId, documentId);
        }

        private async Task<string> GetTitle(int entityId, int documentId)
        {
            return await _unitOfWork.Documents.GetDocumentTitleAsync(entityId, documentId);
        }


        private bool ValidateExtention(IFormFile file)
        {
            var isAllowedExtention = Array.Exists(Constants.AllowedFileTypes, element => element == Path.GetExtension(file.FileName).ToLower());
            if (isAllowedExtention)
            {
                return true;
            }
            return false;
        }

        private bool ValidateFileSize(IFormFile file)
        {

            if (file == null || file.Length == 0 || file.Length > Constants.MAX_FILE_SIZE)
            {
                return false;
            }

            return true;
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        public async Task<DocumentModel> GetStaffProfileImageById(string documentRoot, string tenantId, int staffId)
        {
            DocumentModel model = new DocumentModel();

            var memory = new MemoryStream();
            var staff = await _unitOfWork.Staffusers.GetByIdAsync(staffId);

            if (staff.ProfilePictureId > 0)
            {
                var document = await _unitOfWork.Documents.GetDocumentByIdAsync(staff.ProfilePictureId);
                if(document != null)
                {
                    var filePath = documentRoot + document.FilePath + "\\" + document.FileName;
                    model.ContentType = document.ContentType;
                    model.FileName = document.FileName;
                    model.DisplayFileName = document.DisplayFileName;
                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    model.Document = memory.ToArray();
                }
                else
                {
                    //Read Default
                    var fileName = "b5fd8dcbc82349fe9e59637e18790367.jpg"; //TODO: AKS This needs to come from config
                    var filePath = documentRoot + "\\Documents\\StaffId-0\\" + fileName; //TODO: AKS This needs to come from config
                    model.ContentType = "image/jpeg";
                    model.FileName = "profile.jpg";
                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    model.Document = memory.ToArray();
                }
            }
            else
            {
                //Read Default
                var fileName = "b5fd8dcbc82349fe9e59637e18790367.jpg"; //TODO: AKS This needs to come from config
                var filePath = documentRoot + "\\Documents\\StaffId-0\\" + fileName; //TODO: AKS This needs to come from config
                model.ContentType = "image/jpeg";
                model.FileName = "profile.jpg";
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                model.Document = memory.ToArray();
            }
            return model;
        }

        public async Task<bool> DeleteStaffProfileImageById(int staffId, string rootFolder, string tenantId)
        {
            var staff = await _unitOfWork.Staffusers.GetByIdAsync(staffId);


            if (staff != null)
            {
                int imageId = staff.ProfilePictureId;

                if (imageId > 0)
                {
                    var picture = await _unitOfWork.Documents.GetDocumentByIdAsync(imageId);
                    if (picture != null)
                    {
                        string personFolderPath = $"\\Documents\\{tenantId}\\Staff\\StaffId-{staffId}";
                        string mappedPath = Path.GetFullPath(rootFolder + personFolderPath);
                        string fileToDeleteabsolutePath = Path.GetFullPath(Path.Combine(mappedPath, picture.FileName));
                        if (File.Exists(fileToDeleteabsolutePath))
                        {
                            // If file found, delete it    
                            File.Delete(fileToDeleteabsolutePath);
                        }
                        _unitOfWork.Documents.Remove(picture);
                        staff.ProfilePictureId = 0;
                        _unitOfWork.Staffusers.Update(staff);
                        await _unitOfWork.CommitAsync();

                        //Update member / staff profile picture to sociable Profile
                        int organizationId = staff.OrganizationId;
                        var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(organizationId);
                        var staffUser = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                        if (configuration.SociableSyncEnabled == (int)Status.Active && staffUser != null)
                        {
                            try
                            {
                                DocumentModel document = new DocumentModel();
                                document.StaffId = staffId;
                                var memory = new MemoryStream();

                                var fileName = "b5fd8dcbc82349fe9e59637e18790367.jpg"; //TODO: AKS This needs to come from config
                                var filePath = rootFolder + "\\Documents\\StaffId-0\\" + fileName; //TODO: AKS This needs to come from config
                                document.ContentType = "image/jpeg";
                                document.DisplayFileName = "default_profile.jpg";
                                document.OrganizationId = organizationId;

                                using (var stream = new FileStream(filePath, FileMode.Open))
                                {
                                    await stream.CopyToAsync(memory);
                                }
                                memory.Position = 0;
                                document.Document = memory.ToArray();
                                return await _sociableService.UpdateProfileImage(document);

                            }

                            catch (Exception ex)
                            {
                                _logger.LogError($"Update Staff Profile Picture: Failed to update profile picture: {staff.UserName} failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                            }
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> DeleteEventImageById(int eventId, string rootFolder, string tenantId)
        {
            var eventDetails = await _unitOfWork.Events.GetByIdAsync(eventId);


            if (eventDetails != null)
            {
                int imageId = eventDetails.EventImageId ?? 0;

                if (imageId > 0)
                {
                    var picture = await _unitOfWork.Documents.GetDocumentByIdAsync(imageId);
                    if (picture != null)
                    {
                        string personFolderPath = $"\\Documents\\{tenantId}\\Event\\EventId-{eventId}";
                        string mappedPath = Path.GetFullPath(rootFolder + personFolderPath);
                        string fileToDeleteabsolutePath = Path.GetFullPath(Path.Combine(mappedPath, picture.FileName));
                        if (File.Exists(fileToDeleteabsolutePath))
                        {
                            // If file found, delete it    
                            File.Delete(fileToDeleteabsolutePath);
                        }
                        _unitOfWork.Documents.Remove(picture);
                        eventDetails.EventImageId = 0;
                        _unitOfWork.Events.Update(eventDetails);
                        await _unitOfWork.CommitAsync();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> DeleteEventBannerImageById(int eventId, string rootFolder, string tenantId)
        {
            var eventDetails = await _unitOfWork.Events.GetByIdAsync(eventId);


            if (eventDetails != null)
            {
                int imageId = eventDetails.EventBannerImageId ?? 0;

                if (imageId > 0)
                {
                    var picture = await _unitOfWork.Documents.GetDocumentByIdAsync(imageId);
                    if (picture != null)
                    {
                        string personFolderPath = $"\\Documents\\{tenantId}\\Event\\EventId-{eventId}\\EventBannerImage-{eventId}";
                        string mappedPath = Path.GetFullPath(rootFolder + personFolderPath);
                        string fileToDeleteabsolutePath = Path.GetFullPath(Path.Combine(mappedPath, picture.FileName));
                        if (File.Exists(fileToDeleteabsolutePath))
                        {
                            // If file found, delete it    
                            File.Delete(fileToDeleteabsolutePath);
                        }
                        _unitOfWork.Documents.Remove(picture);
                        eventDetails.EventBannerImageId = 0;
                        _unitOfWork.Events.Update(eventDetails);
                        await _unitOfWork.CommitAsync();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<DocumentModel> GetEventImageById(string documentRoot, string tenantId, int eventId)
        {
            DocumentModel model = new DocumentModel();

            var memory = new MemoryStream();
            var eventDetails = await _unitOfWork.Events.GetByIdAsync(eventId);

            if (eventDetails.EventImageId > 0)
            {
                var document = await _unitOfWork.Documents.GetDocumentByIdAsync(eventDetails.EventImageId ?? 0);
                var filePath = documentRoot + document.FilePath + "\\" + document.FileName;
                model.ContentType = document.ContentType;
                model.FileName = document.FileName;
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                model.Document = memory.ToArray();
            }
            return model;
        }

        public async Task<DocumentModel> GetEventCoverImageById(string documentRoot, string tenantId, int eventId)
        {
            DocumentModel model = new DocumentModel();

            var memory = new MemoryStream();
            var eventDetails = await _unitOfWork.Events.GetByIdAsync(eventId);

            if (eventDetails.EventBannerImageId > 0)
            {
                var document = await _unitOfWork.Documents.GetDocumentByIdAsync(eventDetails.EventBannerImageId ?? 0);
                var filePath = documentRoot + document.FilePath + "\\" + document.FileName;
                model.ContentType = document.ContentType;
                model.FileName = document.FileName;
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                model.Document = memory.ToArray();
            }
            return model;
        }

        public async Task<bool> CloneEventImageById(int cloneEventId, int masterEventId)
        {
            var response = false;
            try
            {
                var eventDetails = await _unitOfWork.Events.GetByIdAsync(masterEventId);

                if (eventDetails.EventImageId > 0)
                {
                    var masterDocument = await _unitOfWork.Documents.GetDocumentByIdAsync(eventDetails.EventImageId ?? 0);
                    Document cloneDocument = new Document();
                    cloneDocument.EventId = cloneEventId;
                    cloneDocument.ContentType = masterDocument.ContentType;
                    cloneDocument.Title = masterDocument.Title;
                    cloneDocument.FileName = masterDocument.FileName;
                    cloneDocument.DisplayFileName = masterDocument.DisplayFileName;

                    string masterPath = masterDocument.FilePath;
                    string masterFolderPath = Path.GetFullPath(_appEnvironment.ContentRootPath + masterPath);
                    string masterFilePath = masterFolderPath + "\\" + masterDocument.FileName;

                    string clonePath = masterDocument.FilePath.Replace($"EventId-{masterEventId}", $"EventId-{cloneEventId}");
                    string cloneFolderPath = Path.GetFullPath(_appEnvironment.ContentRootPath + clonePath);
                    string cloneFilePath = cloneFolderPath + "\\" + cloneDocument.FileName;

                    // If directory does not exist, create it. 
                    if (Directory.Exists(masterFolderPath))
                    {
                        if (!Directory.Exists(cloneFolderPath))
                        {
                            Directory.CreateDirectory(cloneFolderPath);
                        }
                        if (File.Exists(masterFilePath))
                        {
                            File.Copy(masterFilePath, cloneFilePath);
                        }
                        else
                        {
                            throw new FileNotFoundException("File not found");
                        }
                        cloneDocument.FilePath = clonePath;
                        await _unitOfWork.Documents.AddAsync(cloneDocument);
                        await _unitOfWork.CommitAsync();

                        var cloneEvent = await _unitOfWork.Events.GetByIdAsync(cloneEventId);
                        cloneEvent.EventImageId = cloneDocument.DocumentId;
                        _unitOfWork.Events.Update(cloneEvent);
                        await _unitOfWork.CommitAsync();
                        response = true;
                    }
                    else
                    {
                        throw new FileNotFoundException("File not found");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return response;
        }


        public async Task<bool> CloneEventCoverImageById(int cloneEventId, int masterEventId)
        {
            var response = false;
            try
            {
                var eventDetails = await _unitOfWork.Events.GetByIdAsync(masterEventId);
                if (eventDetails.EventBannerImageId > 0)
                {

                    var masterDocument = await _unitOfWork.Documents.GetDocumentByIdAsync(eventDetails.EventBannerImageId ?? 0);
                    Document cloneDocument = new Document();
                    cloneDocument.EventId = cloneEventId;
                    cloneDocument.ContentType = masterDocument.ContentType;
                    cloneDocument.Title = masterDocument.Title;
                    cloneDocument.FileName = masterDocument.FileName;
                    cloneDocument.DisplayFileName = masterDocument.DisplayFileName;

                    string masterPath = masterDocument.FilePath;
                    string masterFolderPath = Path.GetFullPath(_appEnvironment.ContentRootPath + masterPath);
                    string masterFilePath = masterFolderPath + "\\" + masterDocument.FileName;

                    string clonePath = masterDocument.FilePath.Replace($"EventId-{masterEventId}", $"EventId-{cloneEventId}");
                    string cloneFolderPath = Path.GetFullPath(_appEnvironment.ContentRootPath + clonePath);
                    string cloneFilePath = cloneFolderPath + "\\" + cloneDocument.FileName;

                    // If directory does not exist, create it. 
                    if (Directory.Exists(masterFolderPath))
                    {
                        if (!Directory.Exists(cloneFolderPath))
                        {
                            Directory.CreateDirectory(cloneFolderPath);
                        }
                        if (File.Exists(masterFilePath))
                        {
                            File.Copy(masterFilePath, cloneFilePath);
                        }
                        else
                        {
                            throw new FileNotFoundException("File not found");
                        }
                        cloneDocument.FilePath = clonePath;
                        await _unitOfWork.Documents.AddAsync(cloneDocument);
                        await _unitOfWork.CommitAsync();

                        var cloneEvent = await _unitOfWork.Events.GetByIdAsync(cloneEventId);
                        cloneEvent.EventBannerImageId = cloneDocument.DocumentId;
                        _unitOfWork.Events.Update(cloneEvent);
                        await _unitOfWork.CommitAsync();
                        response = true;
                    }
                    else
                    {
                        throw new FileNotFoundException("File not found");
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return response;
        }


        public async Task<Document> CreateDocument(DocumentModel model)
        {
            Document document = new Document();

            string root = model.DocumentRoot;
            string personFolderPath = "";
            string staffFolderPath = "";
            string mappedPath = "";
            byte[] imageBlob = Convert.FromBase64String(model.ProfileImageData);

            if (model.EntityId > 0)
            {
                personFolderPath = $"\\Documents\\{model.TenantId}\\EntityId-{model.EntityId}";
                mappedPath = Path.GetFullPath(root + personFolderPath);
            }

            else if (model.StaffId > 0)
            {
                staffFolderPath = $"\\Documents\\{model.TenantId}\\Staff\\StaffId-{model.StaffId}";
                mappedPath = Path.GetFullPath(root + staffFolderPath);
            }

            // If directory does not exist, create it. 
            if (!Directory.Exists(mappedPath))
            {
                Directory.CreateDirectory(mappedPath);
            }

            string extention = Path.GetExtension(model.DisplayFileName);
            string fileName = Guid.NewGuid().ToString().Replace("-", "") + extention;

            var isAllowedExtention = Array.Exists(Constants.AllowedFileTypes, element => element == extention);
            if (!isAllowedExtention)
            {
                return document;
            }

            try
            {
                string absolutePath = Path.GetFullPath(Path.Combine(mappedPath, fileName));
                using (var stream = File.Create(absolutePath))
                {
                    if (model.EntityId > 0)
                    {
                        document.EntityId = model.EntityId;
                        document.FilePath = personFolderPath;
                    }

                    if (model.StaffId > 0)
                    {
                        document.StaffId = model.StaffId;
                        document.FilePath = staffFolderPath;
                    }

                    document.OrganizationId = model.OrganizationId;
                    document.FileName = fileName;
                    document.DisplayFileName = model.DisplayFileName;
                    document.ContentType = model.ContentType;
                    document.Title = "";

                    if (model.EntityId > 0)
                    {
                        var picture = await _unitOfWork.Documents.GetPictureByEntityIdAsync(model.EntityId);
                        if (picture != null)
                        {
                            string fileToDeleteabsolutePath = Path.GetFullPath(Path.Combine(mappedPath, picture.FileName));
                            if (File.Exists(fileToDeleteabsolutePath))
                            {
                                // If file found, delete it    
                                File.Delete(fileToDeleteabsolutePath);
                            }
                            _unitOfWork.Documents.Remove(picture);
                            await _unitOfWork.Documents.AddAsync(document);
                        }
                        else
                        {
                            await _unitOfWork.Documents.AddAsync(document);

                        }
                        await _unitOfWork.CommitAsync();
                        stream.Write(imageBlob, 0, imageBlob.Length);


                        var entity = await _unitOfWork.Entities.GetByIdAsync(model.EntityId);
                        entity.ProfilePictureId = document.DocumentId;
                        _unitOfWork.Entities.Update(entity);
                        await _unitOfWork.CommitAsync();
                    }
                    if (model.StaffId > 0)
                    {
                        var picture = await _unitOfWork.Documents.GetProfilePictureByStaffIdAsync(model.StaffId);
                        if (picture != null)
                        {
                            string fileToDeleteabsolutePath = Path.GetFullPath(Path.Combine(mappedPath, picture.FileName));
                            if (File.Exists(fileToDeleteabsolutePath))
                            {
                                // If file found, delete it    
                                File.Delete(fileToDeleteabsolutePath);
                            }
                            _unitOfWork.Documents.Remove(picture);
                            await _unitOfWork.Documents.AddAsync(document);
                        }
                        else
                        {
                            await _unitOfWork.Documents.AddAsync(document);

                        }
                        await _unitOfWork.CommitAsync();
                        stream.Write(imageBlob, 0, imageBlob.Length);

                        var staff = await _unitOfWork.Staffusers.GetByIdAsync(model.StaffId);
                        staff.ProfilePictureId = document.DocumentId;
                        _unitOfWork.Staffusers.Update(staff);
                        await _unitOfWork.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return document;
        }

    }
}
