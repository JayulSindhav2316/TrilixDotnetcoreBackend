using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Max.Services;
using Microsoft.AspNetCore.Http;
using Max.Core.Models;
using System.Linq;
using System;
using Microsoft.Extensions.Hosting;
using Max.Services.Helpers;

namespace Max.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        string rootFolder = string.Empty;
        private readonly ILogger<DocumentController> _logger;
        private readonly IDocumentService _documentService;
        private readonly IHostEnvironment _appEnvironment;
        private readonly IReceiptHeaderService _receiptHeaderService;
        private readonly IInvoiceService _invoiceService;
        private readonly IPaperInvoiceService _paperInvoiceService;
        private readonly IOrganizationService _organizationService;
        [System.Obsolete]
        public DocumentController(
            ILogger<DocumentController> logger,
            IDocumentService DocumentService,
            IHostEnvironment appEnvironment,
            IReceiptHeaderService receiptHeaderService,
            IInvoiceService invoiceService,
            IPaperInvoiceService paperInvoiceService,
            IOrganizationService organizationService)
        {
            _logger = logger;
            _documentService = DocumentService;
            _appEnvironment = appEnvironment;
            _receiptHeaderService = receiptHeaderService;
            _invoiceService = invoiceService;
            _paperInvoiceService = paperInvoiceService;
            _organizationService = organizationService;
            rootFolder = _appEnvironment.ContentRootPath;
        }

        [HttpGet("GetAllDocuments")]
        public async Task<ActionResult<IEnumerable<Document>>> GetAllDocuments()
        {
            var documents = await _documentService.GetAllDocuments();
            return Ok(documents);
        }

        [HttpPost("UploadProfileImage")]
        public async Task<ActionResult<Document>> UploadProfileImage([FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Failed to update Person" });
            }
            var formCollection = await Request.ReadFormAsync();
            var upLoadFile = formCollection.Files.First();

            if (formCollection.Count == 0)
            {
                return BadRequest(new { message = "Data is Missing." });
            }
            int? entityId = null;
            int? organizationId = null;
            string tenantId = string.Empty;
            int? staffId = null;

            var entityKeys = formCollection["entityId"];
            if (entityKeys.Count != 0)
            {
                entityId = int.Parse(formCollection["entityId"][0]);
            }

            var organizationKeys = formCollection["organizationId"];
            if (organizationKeys.Count != 0)
            {
                organizationId = int.Parse(formCollection["organizationId"][0]);
            }

            var staffKeys = formCollection["staffId"];
            if (staffKeys.Count != 0)
            {
                staffId = int.Parse(formCollection["staffId"][0]);
            }

            tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();

            string organizationImageType = formCollection["imageType"].Count == 0 ? "" : formCollection["imageType"][0].ToString();

            DocumentModel model = new DocumentModel();
            model.EntityId = entityId ?? 0;
            model.OrganizationId = organizationId;
            model.Title = organizationImageType;
            model.TenantId = tenantId;
            model.StaffId = staffId ?? 0;
            model.DocumentRoot = _appEnvironment.ContentRootPath;

            try
            {
                var document = await _documentService.CreateDocument(upLoadFile, model);
                return Ok(document);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetInvoicePdf")]
        public async Task<FileResult> GetInvoicePdf(int invoiceId)
        {
            rootFolder = _appEnvironment.ContentRootPath;
            var invoiceModel = await _invoiceService.GetInvoiceDetailsByInvoiceId(invoiceId);
            DocumentModel model = new DocumentModel();
            PdfInvoice pdfReceipt = new PdfInvoice(rootFolder, invoiceModel);
            model.Document = pdfReceipt.GetPdf();
            model.FileName = "Invoice-" + invoiceModel.InvoiceId.ToString() + ".pdf";
            model.ContentType = "application/pdf";

            return File(model.Document, model.ContentType, model.FileName);
        }

        [HttpGet("GetReceiptPdf")]
        public async Task<FileResult> GetReceiptPdf(int cartId = 0, int receiptId = 0)
        {
            rootFolder = _appEnvironment.ContentRootPath;
            ReceiptModel receiptModel = new ReceiptModel();

            if (receiptId > 0)
            {
                receiptModel = await _receiptHeaderService.GetReceiptDetailByReceiptId(receiptId);
            }
            if (cartId > 0)
            {
                receiptModel = await _receiptHeaderService.GetReceiptDetailByCartId(cartId);
            }


            DocumentModel model = new DocumentModel();
            CreatePdfReceipt pdfReceipt = new CreatePdfReceipt(rootFolder, receiptModel);
            model.Document = pdfReceipt.GetPdf();
            model.FileName = "Receipt-" + receiptModel.Receiptid.ToString() + ".pdf";
            model.ContentType = "application/pdf";

            return File(model.Document, model.ContentType, model.FileName);
        }
        [HttpGet("GetPaperInvoicePdf")]
        public async Task<FileResult> GetPaperInvoicePdf(int cycleId = 0, int organizationId = 0)
        {
            rootFolder = _appEnvironment.ContentRootPath;
            List<InvoiceModel> invoices = new List<InvoiceModel>();
            var organization = await _organizationService.GetOrganizationById(organizationId);

            if (cycleId > 0)
            {
                invoices = (List<InvoiceModel>)await _paperInvoiceService.GetManualBillingInvoices(cycleId);
            }

            DocumentModel model = new DocumentModel();
            CreatePaperInvoice paperInvoicePdf = new CreatePaperInvoice(rootFolder, invoices, organization);
            model.Document = paperInvoicePdf.GetPdf();
            model.FileName = "PaperInvoices-" + cycleId.ToString() + ".pdf";
            model.ContentType = "application/pdf";

            return File(model.Document, model.ContentType, model.FileName);
        }

        [HttpGet("GetPaperInvoicePdfByInvoiceId")]
        public async Task<FileResult> GetPaperInvoicePdfByInvoiceId(int invoiceId = 0, int organizationId = 0)
        {
            rootFolder = _appEnvironment.ContentRootPath;
            List<InvoiceModel> invoices = new List<InvoiceModel>();
            var organization = await _organizationService.GetOrganizationById(organizationId);

            if (invoiceId > 0)
            {
                var invoice = await _paperInvoiceService.GetManualBillingInvoiceByInvoiceId(invoiceId);
                if (invoice != null)
                {
                    invoices.Add(invoice);
                }
            }

            DocumentModel model = new DocumentModel();
            CreatePaperInvoice paperInvoicePdf = new CreatePaperInvoice(rootFolder, invoices, organization);
            model.Document = paperInvoicePdf.GetPdf();
            model.FileName = "PaperInvoice-" + invoiceId.ToString() + ".pdf";
            model.ContentType = "application/pdf";

            return File(model.Document, model.ContentType, model.FileName);
        }

        [HttpGet("GetEventPaperInvoicePdfByInvoiceId")]
        public async Task<FileResult> GetEventPaperInvoicePdfByInvoiceId(int invoiceId = 0, int organizationId = 0)
        {
            rootFolder = _appEnvironment.ContentRootPath;
            List<InvoiceModel> invoices = new List<InvoiceModel>();
            var organization = await _organizationService.GetOrganizationById(organizationId);

            if (invoiceId > 0)
            {
                var invoice = await _paperInvoiceService.GetManualBillingInvoiceByInvoiceId(invoiceId);
                if (invoice != null)
                {
                    invoices.Add(invoice);
                }
            }

            DocumentModel model = new DocumentModel();
            CreatePaperInvoice paperInvoicePdf = new CreatePaperInvoice(rootFolder, invoices, organization);
            model.Document = paperInvoicePdf.GetEventPdf();
            model.FileName = "PaperInvoice-" + invoiceId.ToString() + ".pdf";
            model.ContentType = "application/pdf";

            return File(model.Document, model.ContentType, model.FileName);
        }

        [HttpGet("GetProfileImage")]
        public async Task<ActionResult> GetProfileImage(int entityId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Invalid model");

            }
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            rootFolder = _appEnvironment.ContentRootPath;

            try
            {
                var document = await _documentService.GetProfileImageById(rootFolder, tenantId, entityId);

                return File(document.Document, document.ContentType, document.FileName);
            }
            catch(Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest();
            }
        }


        [HttpGet("GetOrganizationImage")]
        public async Task<FileResult> GetOrganizationImage(int organizationId, string title)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Invalid model");

            }
            rootFolder = _appEnvironment.ContentRootPath;

            var document = await _documentService.GetOrganizationImageByOrgIdAndTitle(rootFolder, organizationId, title);

            return File(document.Document, document.ContentType, document.FileName);
        }
        [HttpGet("GetOrganizationImageInBase64")]
        public async Task<ActionResult<string>> GetOrganizationImageInBase64(int organizationId, string title)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Invalid model");

            }
            var rootFolder = _appEnvironment.ContentRootPath;

            var document = await _documentService.GetOrganizationImageByOrgIdAndTitle(rootFolder, organizationId, title);

            string base64String = Convert.ToBase64String(document.Document);

            base64String = "data:image/jpg;base64," + base64String;
            return Ok(base64String);
        }

        [HttpDelete("DeleteProfileImage/{entityId}")]
        public async Task<ActionResult<bool>> DeleteProfileImage(int entityId)
        {
            bool response = false;
            try
            {
                response = await _documentService.DeleteProfileImageByEntityId(entityId, _appEnvironment.ContentRootPath);
                if (!response)
                {
                    return BadRequest(new { message = "Profile photo does not exist." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpGet("GetStaffProfileImage")]
        public async Task<FileResult> GetStaffProfileImage(int staffId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Invalid model");

            }
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            rootFolder = _appEnvironment.ContentRootPath;

            var document = await _documentService.GetStaffProfileImageById(rootFolder, tenantId, staffId);

            return File(document.Document, document.ContentType, document.FileName);


        }

        [HttpDelete("DeleteStaffProfileImage/{staffId}")]
        public async Task<ActionResult<bool>> DeleteStaffProfileImage(int staffId)
        {
            bool response = false;
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            try
            {
                response = await _documentService.DeleteStaffProfileImageById(staffId, _appEnvironment.ContentRootPath, tenantId);
                if (!response)
                {
                    return BadRequest(new { message = "Profile photo does not exist." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UploadOrganizationImage")]
        public async Task<ActionResult<Document>> UploadOrganizationImage([FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Failed to upload image" });
            }
            var formCollection = await Request.ReadFormAsync();

            if (formCollection.Count == 0)
            {
                return BadRequest(new { message = "Data is Missing." });
            }

            int? organizationId = null;
            string tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();

            var organizationKeys = formCollection["organizationId"];
            if (organizationKeys.Count != 0)
            {
                organizationId = int.Parse(formCollection["organizationId"][0]);
            }

            foreach (var _file in formCollection.Files)
            {
                string organizationImageType = _file.Name;

                DocumentModel model = new DocumentModel();
                model.EntityId = 0;
                model.OrganizationId = organizationId;
                model.Title = organizationImageType;
                model.TenantId = tenantId;
                model.StaffId = 0;
                model.DocumentRoot = _appEnvironment.ContentRootPath;

                try
                {
                    var document = await _documentService.CreateDocument(_file, model);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            return Ok(true);
        }

        [HttpPost("UploadEventImage")]
        public async Task<ActionResult<Document>> UploadEventImage([FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Failed to upload image" });
            }
            var formCollection = await Request.ReadFormAsync();
            var uploadFile = formCollection.Files.First();

            if (formCollection.Count == 0)
            {
                return BadRequest(new { message = "Data is Missing." });
            }

            int eventId = 0;
            string tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();

            var keys = formCollection["eventId"];
            if (keys.Count != 0)
            {
                eventId = int.Parse(formCollection["eventId"][0]);
            }


            string organizationImageType = formCollection.Files[0].Name;

            DocumentModel model = new DocumentModel();
            model.EntityId = 0;
            model.EventId = eventId;
            model.Title = "Event Image";
            model.TenantId = tenantId;
            model.StaffId = 0;
            model.DocumentRoot = _appEnvironment.ContentRootPath;

            try
            {
                var document = await _documentService.CreateDocument(uploadFile, model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }

        [HttpPost("UploadEventBannerImage")]
        public async Task<ActionResult<Document>> UploadEventBannerImage([FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Failed to upload image" });
            }
            var formCollection = await Request.ReadFormAsync();
            var uploadFile = formCollection.Files.First();

            if (formCollection.Count == 0)
            {
                return BadRequest(new { message = "Data is Missing." });
            }

            int eventId = 0;
            int eventBannerImageId = 0;
            string tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();

            var keys = formCollection["eventId"];
            if (keys.Count != 0)
            {
                eventId = int.Parse(formCollection["eventId"][0]);
                eventBannerImageId = int.Parse(formCollection["eventBannerImageId"][0]);
            }

            string organizationImageType = formCollection.Files[0].Name;

            DocumentModel model = new DocumentModel();
            model.EntityId = 0;
            model.EventId = eventId;
            model.EventBannerImageId = eventBannerImageId;
            model.Title = "Event Banner Image";
            model.TenantId = tenantId;
            model.StaffId = 0;
            model.DocumentRoot = _appEnvironment.ContentRootPath;

            try
            {
                var document = await _documentService.CreateDocument(uploadFile, model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }

        [HttpDelete("DeleteEventImage/{eventId}")]
        public async Task<ActionResult<bool>> DeleteEventImage(int eventId)
        {
            bool response = false;
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            try
            {
                response = await _documentService.DeleteEventImageById(eventId, _appEnvironment.ContentRootPath, tenantId);
                if (!response)
                {
                    return BadRequest(new { message = "Event Image does not exist." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpDelete("DeleteEventCoverImage/{eventId}")]
        public async Task<ActionResult<bool>> DeleteEventCoverImage(int eventId)
        {
            bool response = false;
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            try
            {
                response = await _documentService.DeleteEventBannerImageById(eventId, _appEnvironment.ContentRootPath, tenantId);
                if (!response)
                {
                    return BadRequest(new { message = "Event Image does not exist." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpGet("GetEventImage")]
        public async Task<FileResult> GetEventImage(int eventId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Invalid model");

            }
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            rootFolder = _appEnvironment.ContentRootPath;

            var document = await _documentService.GetEventImageById(rootFolder, tenantId, eventId);

            return File(document.Document, document.ContentType, document.FileName);
        }

        [HttpGet("GetEventCoverImage")]
        public async Task<FileResult> GetEventCoverImage(int eventId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Invalid model");

            }
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            rootFolder = _appEnvironment.ContentRootPath;

            var document = await _documentService.GetEventCoverImageById(rootFolder, tenantId, eventId);

            return File(document.Document, document.ContentType, document.FileName);
        }

        [HttpPost("UploadProfileImageFromSociable")]
        public async Task<ActionResult<Document>> UploadMemberProfileImageFromSociable(DocumentModel documentModel)
        {

            documentModel.TenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            documentModel.DocumentRoot = _appEnvironment.ContentRootPath;

            try
            {
                var document = await _documentService.CreateDocument(documentModel);
                return Ok(document);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
