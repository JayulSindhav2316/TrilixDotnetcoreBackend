using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Max.Core;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DocumentContainerController : ControllerBase
    {
        string rootFolder = string.Empty;
        private readonly ILogger<EntityController> _logger;
        private readonly IDocumentContainerService _documentContainerService;
        private readonly IHostEnvironment _appEnvironment;
       
        public DocumentContainerController(ILogger<EntityController> logger, IDocumentContainerService documentContainerService, IHostEnvironment appEnvironment)
        {
            _logger = logger;
            _documentContainerService = documentContainerService;
            _appEnvironment = appEnvironment;
            rootFolder = _appEnvironment.ContentRootPath;
        }

        [HttpPost("CreateContainer")]
        public async Task<ActionResult<DocumentContainerModel>> CreateContainer([FromBody] ContainerRequestModel model )
        {
            var container = new DocumentContainerModel();
            try
            {
                container = await _documentContainerService.CreateDocumentContainer(model);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(container);
        }

        [HttpPost("CreateFolder")]
        public async Task<ActionResult<DocumentObjectModel>> CreateFolder([FromBody] DocumentObjectModel model)
        {
            var folder = new DocumentObjectModel();
            try
            {
                folder = await _documentContainerService.CreateFolder(model);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(folder);
        }

        [HttpPost("UpdateContainer")]
        public async Task<ActionResult<DocumentContainerModel>> UpdateContainer([FromBody] ContainerRequestModel model)
        {
            var container = new DocumentContainerModel();
            try
            {
                container = await _documentContainerService.UpdateDocumentContainer(model);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(container);
        }

        [HttpPost("DeleteContainer")]
        public async Task<ActionResult<bool>> DeleteContainer([FromBody] ContainerRequestModel model)
        {
            var deleted  = false;
            try
            {
                deleted = await _documentContainerService.DeleteDocumentContainer(model);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(deleted);
        }

        [HttpGet("GetContainerList")]
        public async Task<ActionResult<List<DocumentContainerModel>>> GetContainerList()
        {
            var containerList = new List<DocumentContainerModel>();
            try
            {
                containerList = await _documentContainerService.GetAllDocumentContainers();

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(containerList);
        }
        [HttpGet("GetContainerById")]
        public async Task<ActionResult<DocumentContainerModel>> GetContainerById(int containerId)
        {
            var container = new DocumentContainerModel();
            try
            {
                container = await _documentContainerService.GetDocumentContainerById(containerId);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(container);
        }
        [HttpGet("GetContainerByFolderKey")]
        public async Task<ActionResult<DocumentContainerModel>> GetContainerByFolderKey(string key)
        {
            var container = new DocumentContainerModel();
            try
            {
                container = await _documentContainerService.GetDocumentContainerByFolderKey(key);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(container);
        }

        [HttpGet("GetContainerAccessListByContainerId")]
        public async Task<ActionResult<List<ContainerAccessModel>>> GetContainerAccessListByContainerId(int containerId)
        {
            var accessList = new List<ContainerAccessModel>();
            if(containerId > 0)
            {
                try
                {
                    accessList = await _documentContainerService.GetContainerAccessListByContainerId(containerId);

                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            return Ok(accessList);
        }
        [HttpGet("GetDocumentAccessListById")]
        public async Task<ActionResult<List<DocumentAccessModel>>> GetDocumentAccessListById(int documentObjectId)
        {
            var accessList = new List<DocumentAccessModel>();
            if (documentObjectId > 0)
            {
                try
                {
                    accessList = await _documentContainerService.GetDocumentAccessListByDocumentId(documentObjectId);

                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            return Ok(accessList);
        }
        [HttpGet("GetDocumentTagListById")]
        public async Task<ActionResult<List<DocumentTagModel>>> GetDocumentTagListById(int documentObjectId)
        {
            var tagList = new List<DocumentTagModel>();
            if (documentObjectId > 0)
            {
                try
                {
                    tagList = await _documentContainerService.GetDocumentTagListById(documentObjectId);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            return Ok(tagList);
        }
        [HttpGet("GetRootContainerTree")]
        public async Task<ActionResult<List<ContainerTreeModel>>> GetRootContainerTree(string selectedNode,int? entityId)
        {
            var containerTree = new List<ContainerTreeModel>();
            try
            {
                containerTree = await _documentContainerService.GetDocumentContainerTree(entityId,"", selectedNode);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(containerTree);
        }
        [HttpGet("GetDocumentsByContainerAndPath")]
        public async Task<ActionResult<List<ContainerTreeModel>>> GetDocumentsByContainerAndPath(int id, string path, int? entityId)
        {
            var documents = new List<DocumentObjectModel>();
            try
            {
                documents = await _documentContainerService.GetDocumentObjectsByContainerAndPath(id, path, entityId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(documents);
        }

        [HttpGet("GetDocumentsByText")]
        public async Task<ActionResult<SolrSearchResultModel>> GetDocumentsByText(string text, DateTime fromDate, DateTime toDate, int staffUserId = 0, int entityId=0, string searchFilter = "", string tags = "", int startPage=0, string sortBy="", string tenantId="")
        {
            _logger.LogInformation($"tenantId:{tenantId} text:{text} fromDate:{fromDate} toDate: {toDate} staffUserId: {staffUserId} entityId: {entityId} searchFilter: {searchFilter} tags: {tags} startPage: {startPage} sortBy: {sortBy}"  );
            var serachResult = new SolrSearchResultModel();
            try
            {
                if(searchFilter == null)
                {
                    searchFilter = string.Empty;
                }
                if (sortBy == null)
                {
                    sortBy = string.Empty;
                }
                if (tags == null || tags =="undefined")
                {
                    tags = string.Empty;
                }
                serachResult =  await _documentContainerService.GetDocumentObjectsByText(text, searchFilter, tags, fromDate, toDate, entityId, startPage, sortBy, staffUserId,tenantId);

            }
            catch (Exception ex)
            {
                _logger.LogError($"GetDocumentsByText: {ex.Message} {ex.StackTrace} {ex.InnerException} {ex.Source}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(serachResult);
        }

        [HttpPost("UploadDocument")]
        public async Task<ActionResult<DocumentObjectModel>> UploadDocument([FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Failed to upload document" });
            }
            rootFolder = _appEnvironment.ContentRootPath;
            var formCollection = await Request.ReadFormAsync();
            var upLoadFile = formCollection.Files.First();

            if (formCollection.Count == 0)
            {
                return BadRequest(new { message = "Data is Missing." });
            }

            int containerId = 0;
            int staffId = 0;
            string pathName = string.Empty;
            string fileName = string.Empty;
            int organizationId = 0;
            string tenantId = string.Empty;
            int entityId = 0;
            var entityKeys = formCollection["containerId"];
            if (entityKeys.Count != 0)
            {
                containerId = int.Parse(formCollection["containerId"][0]);
            }

            var staffKeys = formCollection["staffId"];
            if (staffKeys.Count != 0)
            {
                staffId = int.Parse(formCollection["staffId"][0]);
            }

            var tenant = formCollection["tenantId"];
            if (tenant.Count != 0)
            {
                tenantId = formCollection["tenantId"][0];
            }

            var filePath = formCollection["filePath"];
            if (filePath.Count != 0)
            {
                pathName = formCollection["filePath"][0];
            }
            var organization = formCollection["organizationId"];
            if (organization.Count != 0)
            {
                organizationId = int.Parse(formCollection["organizationId"][0]);
            }

            var entity = formCollection["entityId"];
            if(entity.Count != 0)
            {
                entityId = int.Parse(formCollection["entityId"][0]);
            }

            var tags = formCollection["tags"];
            DocumentObjectModel model = new DocumentObjectModel();
            model.ContainerId = containerId;
            model.CreatedBy = staffId;
            model.PathName = pathName;
            model.FileName = upLoadFile.FileName;
            model.FileType = (int)DocumentObjectType.File;
            model.TempRootFolder = _appEnvironment.ContentRootPath;
            model.OrganizationId = organizationId;
            model.TenantId = tenantId;
            model.entityId = entityId;
            model.SelectedTags = JsonConvert.DeserializeObject<List<SelectListModel>>(tags);
            try
            {
                var document = await _documentContainerService.CreateDocumentObject(upLoadFile, model);
                return Ok(document);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("DownloadDocument")]
        public async Task<FileResult> DownloadDocument(int documentObjectId=0, int staffUserId=0, int entityId=0, int organizationId=0,string tenantId="")
        {
            var request = new DocumentObjectRequestModel();

            request.DocumentObjectId = documentObjectId;
            request.StaffId = staffUserId;
            request.EntityId = entityId;
            request.OrganizationId = organizationId;
            request.TenantId = tenantId;

            if (staffUserId == 0 && entityId == 0)
            {
                throw new UnauthorizedAccessException("Access is denied");
            }
            else
            {
                var document = new DocumentObjectResponseModel();
                rootFolder = _appEnvironment.ContentRootPath;
                request.TempRootFolder = rootFolder;

                document = await _documentContainerService.GetDocumentObject(request);
                if (document.ResponseMessage == "Access Denied")
                {
                    throw new UnauthorizedAccessException("Access is denied");
                }
                else
                {
                    return File(document.Document, document.DocumentType, document.DocumentName);
                }
            }
           
        }
        [HttpGet("GetDocumentUrl")]
        public BadRequestObjectResult GetDocumentUrl(int documentObjectId = 0, int staffUserId = 0, int entityId = 0, int organizationId = 0, string tenantId = "")
        {
            return BadRequest(new { message = "This method is no longer supported." });
        }
        [HttpGet("GetDownloadDocumentObject")]
        public async Task<DocumentObjectResponseModel> GetDownloadDocumentObject(int documentObjectId = 0, int entityId = 0, int staffUserId = 0, int organizationId = 0, string tenantId = "")
        {
            var request = new DocumentObjectRequestModel();

            request.DocumentObjectId = documentObjectId;
            request.EntityId = entityId;
            request.OrganizationId = organizationId;
            request.TenantId = tenantId;
            request.StaffId = staffUserId;

            rootFolder = _appEnvironment.ContentRootPath;
            request.TempRootFolder = rootFolder;
            if (entityId == 0 && staffUserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                var document =  await _documentContainerService.GetDocumentObject(request);
                if (document.ResponseMessage == "Access Denied")
                {
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    return document;
                }
            }

        }

        [HttpGet("GetAuditTrailByDocumentId")]
        public async Task<ActionResult<List<DocumentObjectAccessHistoryModel>>> GetAuditTrailByDocumentId(int documentObjectId = 0)
        {

            try
            {
                var auditTrail = await _documentContainerService.GetAuditTrailByDocumentId(documentObjectId);
                return Ok(auditTrail);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpGet("GetAuditTrail")]
        public async Task<ActionResult<List<DocumentObjectAccessHistoryModel>>> GetAuditTrail(DateTime startDate, DateTime endDate)
        {

            try
            {
                var auditTrail = await _documentContainerService.GetAuditTrailByDateRange(startDate, endDate);
                return Ok(auditTrail);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("DeleteFolder")]
        public async Task<ActionResult<Boolean>> DeleteFolder([FromBody] DocumentObjectModel model)
        {
            try
            {
                await _documentContainerService.DeleteFolder(model);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
        }
        [HttpPost("UpdateFolder")]
        public async Task<ActionResult<DocumentObjectModel>> UpdateFolder([FromBody] DocumentObjectModel model)
        {
            try
            {
                var documentObject = await _documentContainerService.UpdateFolder(model);
                return Ok(documentObject);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("UpdateDocumentTags")]
        public async Task<ActionResult<DocumentObjectModel>> UpdateDocumentTags([FromBody] DocumentObjectModel model)
        {
            try
            {
                var documentObject = await _documentContainerService.UpdateDocumentTags(model);
                return Ok(documentObject);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("UpdateDocumentAccessControl")]
        public async Task<ActionResult<DocumentObjectModel>> UpdateDocumentAccessControl([FromBody] DocumentObjectModel model)
        {
            try
            {
                var documentObject = await _documentContainerService.UpdateDocumentAccessControl(model);
                return Ok(documentObject);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("DeleteDocument")]
        public async Task<ActionResult<Boolean>> DeleteDocument([FromBody] DocumentObjectRequestModel model)
        {
            try
            {
                await _documentContainerService.DeleteDocument(model);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpGet("ExportDocuments")]
        public async Task<ActionResult<List<Boolean>>> ExportDocuments(string  userName)
        {

            try
            {
                var exportResult = await _documentContainerService.ExportDocuments(userName);
                return Ok(exportResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
    }
}
