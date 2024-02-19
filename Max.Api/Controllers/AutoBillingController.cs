using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Api.Helpers;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AutoBillingController : ControllerBase
    {

        private readonly ILogger<AutoBillingController> _logger;
        private readonly IAutoBillingProcessingDatesService _autoBillingProcessingDatesService;
        private readonly IAutoBillingSettingsService _autoBillingSettingsService;
        private readonly IAutoBillingDraftService _autoBillingDraftService;
        private readonly IAutoBillingNotificationService _autoBillingNotificationService;
        private readonly IBillingDocumentsService _billingDocumentsService;
        private readonly IEmailService _emailService;
        private readonly IAutoBillingService _autoBillingService;

        public AutoBillingController(ILogger<AutoBillingController> logger, 
                                        IAutoBillingProcessingDatesService autoBillingProcessingDatesService,
                                        IAutoBillingSettingsService autoBillingSettingsService,
                                        IAutoBillingDraftService autoBillingDraftService,
                                        IAutoBillingNotificationService autoBillingNotificationService,
                                        IBillingDocumentsService billingDocumentsService,
                                        IEmailService emailService,
                                        IAutoBillingService autoBillingService
                                    )
        {
            _logger = logger;
            _autoBillingProcessingDatesService = autoBillingProcessingDatesService;
            _autoBillingSettingsService = autoBillingSettingsService;
            _autoBillingDraftService = autoBillingDraftService;
            _autoBillingNotificationService = autoBillingNotificationService;
            _billingDocumentsService = billingDocumentsService;
            _emailService = emailService;
            _autoBillingService = autoBillingService;
        }

        // Start Autobilling Processing Date Section
        [HttpPost("CreateAutoBillingProcessingDate")]
        public async Task<ActionResult<Autobillingprocessingdate>> CreateAutoBillingProcessingDate(AutoBillingProcessingDateModel autoBillingProcessingDateModel)
        {
            Autobillingprocessingdate response = new Autobillingprocessingdate();
            try
            {
                response = await _autoBillingProcessingDatesService.CreateAutoBillingProcessingDate(autoBillingProcessingDateModel);
                if (response.AutoBillingProcessingDatesId == 0)
                {
                    return BadRequest(new { message = "Failed to create Staff User" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdateAutoBillingProcessingDate")]
        public async Task<ActionResult<Autobillingprocessingdate>> UpdateAutoBillingProcessingDate(AutoBillingProcessingDateModel autoBillingProcessingDateModel)
        {
            bool response = false;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse(406, "Invalid information."));
            }

            try
            {
                response = await _autoBillingProcessingDatesService.UpdateAutoBillingProcessingDate(autoBillingProcessingDateModel);
                if (!response)
                {
                    return BadRequest(new ApiResponse(304, "Record could not be updated."));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(500, "Internal Server error."));
            }

            return Ok(response);
        }

        [HttpGet("GetAllAutoBillingProcessingDates")]
        public async Task<ActionResult<AutoBillingProcessingDateModel>> GetAllAutoBillingProcessingDates()
        {
            var processingDate = await _autoBillingProcessingDatesService.GetAutoBillingProcessingDates();
            return Ok(processingDate);
        }

        [HttpGet("GetAutoBillingProcessingDatesByBillingType")]
        public async Task<ActionResult<AutoBillingProcessingDateModel>> GetAutoBillingProcessingDatesByBillingType( string billingType)
        {
            var processingDate = await _autoBillingProcessingDatesService.GetAutoBillingProcessingDatesByBillingType(billingType);
            return Ok(processingDate);
        }

        [HttpGet("GetAutoBillingProcessingDatesByABPDId")]
        public async Task<ActionResult<AutoBillingProcessingDateModel>> GetAutoBillingProcessingDatesByABPDId(int abpdId)
        {
            var processingDate = await _autoBillingProcessingDatesService.GetAutoBillingProcessingDatesByABPDId(abpdId);
            return Ok(processingDate);
        }

        [HttpGet("GetAutoBillingProcessingDatesByInvoiceType")]
        public async Task<ActionResult<AutoBillingProcessingDateModel>> GetAutoBillingProcessingDatesByInvoiceType(string invoiceType)
        {
            var processingDate = await _autoBillingProcessingDatesService.GetAutoBillingProcessingDatesByInvoiceType(invoiceType);
            return Ok(processingDate);
        }

        [HttpGet("GetAutoBillingProcessingDatesByThroughDate")]
        public async Task<ActionResult<AutoBillingProcessingDateModel>> GetAutoBillingProcessingDatesByThroughDate(DateTime throughDate)
        {
            var processingDate = await _autoBillingProcessingDatesService.GetAutoBillingProcessingDatesByThroughDate(throughDate);
            return Ok(processingDate);
        }

        [HttpGet("GetAutoBillingProcessingDatesByEffectiveDate")]
        public async Task<ActionResult<AutoBillingProcessingDateModel>> GetAutoBillingProcessingDatesByEffectiveDate(DateTime effectiveDate)
        {
            var processingDate = await _autoBillingProcessingDatesService.GetAutoBillingProcessingDatesByEffectiveDate(effectiveDate);
            return Ok(processingDate);
        }

        [HttpGet("GetAutoBillingProcessingDatesByStatus")]
        public async Task<ActionResult<AutoBillingProcessingDateModel>> GetAutoBillingProcessingDatesByStatus(int status)
        {
            var processingDate = await _autoBillingProcessingDatesService.GetAutoBillingProcessingDatesByStatus(status);
            return Ok(processingDate);
        }

        // End Autobilling Processing Date Section

        // Start Autobilling Setting Section

        [HttpPost("CreateAutoBillingSetting")]
        public async Task<ActionResult<Autobillingsetting>> CreateAutoBillingSetting(AutoBillingSettingModel autoBillingSettingModel)
        {
            Autobillingsetting response = new Autobillingsetting();
            try
            {
                response = await _autoBillingSettingsService.CreateAutoBillingSetting(autoBillingSettingModel);
                if (response.AutoBillingsettingsId == 0)
                {
                    return BadRequest(new { message = "Failed to create Staff User" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdateAutoBillingSetting")]
        public async Task<ActionResult<Autobillingsetting>> UpdateAutoBillingSetting(AutoBillingSettingModel autoBillingSettingModel)
        {
            bool response = false;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse(406, "Invalid information."));
            }

            try
            {
                response = await _autoBillingSettingsService.UpdateAutoBillingSetting(autoBillingSettingModel);
                if (!response)
                {
                    return BadRequest(new ApiResponse(304, "Record could not be updated."));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(500, "Internal Server error."));
            }

            return Ok(response);
        }

        [HttpGet("GetAutoBillingConfiguration")]
        public async Task<ActionResult<AutoBillingConfiguration>> GetAutoBillingConfiguration()
        {
            var configuration = await _autoBillingSettingsService.GetAutoBillingConfiguration();
            return Ok(configuration);
        }

        [HttpPost("UpdateAutoBillingConfiguration")]
        public async Task<ActionResult<bool>> UpdateAutoBillingConfiguration(AutoBillingConfiguration model)
        {
            var configuration = await _autoBillingSettingsService.UpdateAutoBillingConfiguration(model);
            //_emailService.Send();
            return Ok(configuration);
        }

        [HttpGet("GetAutoBillingSetting")]
        public async Task<ActionResult<AutoBillingConfiguration>> GetAutoBillingSetting()
        {
            var configuration = await _autoBillingSettingsService.GetAutoBillingSetting();
            return Ok(configuration);
        }

        // End Autobilling Setting Section

        // Start Autobilling draft Section
        [HttpPost("CreateAutoBillingDraft")]
        public async Task<ActionResult<Autobillingdraft>> CreateAutoBillingDraft(AutoBillingDraftModel autoBillingDraftModel)
        {
            Autobillingdraft response = new Autobillingdraft();
            try
            {
                response = await _autoBillingDraftService.CreateAutoBillingDraft(autoBillingDraftModel);
                if (response.AutoBillingDraftId == 0)
                {
                    return BadRequest(new { message = "Failed to create Staff User" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdateAutoBillingDraft")]
        public async Task<ActionResult<Autobillingdraft>> UpdateAutoBillingDraft(AutoBillingDraftModel autoBillingDraftModel)
        {
            bool response = false;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse(406, "Invalid information."));
            }

            try
            {
                response = await _autoBillingDraftService.UpdateAutoBillingDraft(autoBillingDraftModel);
                if (!response)
                {
                    return BadRequest(new ApiResponse(304, "Record could not be updated."));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(500, "Internal Server error."));
            }

            return Ok(response);
        }

        [HttpGet("GetAutobillingDraftById")]
        public async Task<ActionResult<Autobillingdraft>> GetAutobillingDraftById(int autoBillingDraftId)
        {
            var billingdrafts = await _autoBillingDraftService.GetAutobillingDraftById(autoBillingDraftId);
            return Ok(billingdrafts);
        }

        [HttpGet("GetAutobillingCurrentDraft")]
        public async Task<ActionResult<AutoBillingDraftModel>> GetAutobillingCurrentDraft()
        {
            var billingdrafts = await _autoBillingDraftService.GetAutobillingCurrentDraft();
            return Ok(billingdrafts);
        }

        [HttpGet("GetAllAutobillingDrafts")]
        public async Task<ActionResult<Autobillingdraft>> GetAllAutobillingDrafts()
        {
            var billingdrafts = await _autoBillingDraftService.GetAllAutobillingDrafts();
            return Ok(billingdrafts);
        }
        [HttpGet("GetAutobillingDraftsByPersonId")]
        public async Task<ActionResult<Autobillingdraft>> GetAutobillingDraftsByPersonId(int personId)
        {
            var billingdrafts = await _autoBillingDraftService.GetAutobillingDraftsByPersonId(personId);
            return Ok(billingdrafts);
        }
        //[HttpGet("GetAutobillingDraftByPaymentTransactionId")]
        //public async Task<ActionResult<Autobillingdraft>> GetAutobillingDraftByPaymentTransactionId(int paymentTransactionId)
        //{
        //    var billingdrafts = await _autoBillingDraftService.GetAutobillingDraftByPaymentTransactionId(paymentTransactionId);
        //    return Ok(billingdrafts);
        //}
        [HttpGet("GetAutobillingDraftsByProcessStatus")]
        public async Task<ActionResult<Autobillingdraft>> GetAutobillingDraftsByProcessStatus(int processStatus)
        {
            var billingdrafts = await _autoBillingDraftService.GetAutobillingDraftsByProcessStatus(processStatus);
            return Ok(billingdrafts);
        }
        [HttpGet("GetAutobillingDraftsByBillingDocumentId")]
        public async Task<ActionResult<Autobillingdraft>> GetAutobillingDraftsByBillingDocumentId(int billingDocumentId)
        {
            var billingdrafts = await _autoBillingDraftService.GetAutobillingDraftsByBillingDocumentId(billingDocumentId);
            return Ok(billingdrafts);
        }

        [HttpGet("GetCategorySummaryByBillingDocumentId")]
        public async Task<ActionResult<CategoryRevenueModel>> GetCategorySummaryByBillingDocumentId(int billingDocumentId)
        {
            var billingdrafts = await _autoBillingDraftService.GetAutobillingDraftsSummaryByCategoryId(billingDocumentId);
            return Ok(billingdrafts);
        }

        [HttpGet("GetLastAutobillingDraftsAmountCreated")]
        public async Task<ActionResult<decimal?>> GetLastAutobillingDraftsAmountCreated(int billingDocumentId)
        {
            var lastBillingAmountCreated = await _autoBillingDraftService.GetLastAutobillingDraftsAmountCreated(billingDocumentId);
            return Ok(lastBillingAmountCreated);
        }

        [HttpGet("GetLastAutobillingDraftsAmountApproved")]
        public async Task<ActionResult<decimal?>> GetLastAutobillingDraftsAmountApproved(int billingDocumentId)
        {
            var lastBillingAmountApproved = await _autoBillingDraftService.GetLastAutobillingDraftsAmountApproved(billingDocumentId);
            return Ok(lastBillingAmountApproved);
        }

        [HttpGet("GetLastAutoBillingDraftsAmountDeclined")]
        public async Task<ActionResult<decimal?>> GetLastAutoBillingDraftsAmountDeclined(int billingDocumentId)
        {
            var lastBillingAmountDeclined = await _autoBillingDraftService.GetLastAutoBillingDraftsAmountDeclined(billingDocumentId);
            return Ok(lastBillingAmountDeclined);
        }

        [HttpGet("GetLastBillingChartInvoiceChartData")]
        public async Task<ActionResult<dynamic>> GetLastBillingChartInvoiceChartData()
        {
            var chartData = await _autoBillingDraftService.GetLastBillingChartInvoiceChartData();
            return Ok(chartData);
        }


        [HttpPost("SetAutoPayOnHold")]
        public async Task<ActionResult<bool>> SetAutoPayOnHold(AutoBillingHoldRequestModel model)
        {
            bool response = false;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse(406, "Invalid information."));
            }

            try
            {
                response = await _autoBillingDraftService.SetAutoPayOnHold(model);
                if (!response)
                {
                    return BadRequest(new ApiResponse(304, "Record could not be updated."));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(500, "Internal Server error."));
            }

            return Ok(response);
        }

        [HttpPost("ClearAutoPayOnHold")]
        public async Task<ActionResult<bool>> ClearAutoPayOnHold(AutoBillingHoldRequestModel model)
        {
            bool response = false;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse(406, "Invalid information."));
            }

            try
            {
                response = await _autoBillingDraftService.ClearAutoPayOnHold(model);
                if (!response)
                {
                    return BadRequest(new ApiResponse(304, "Record could not be updated."));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(500, "Internal Server error."));
            }

            return Ok(response);
        }

        // End Autobilling draft Section

        // Start Auto Billing NotifiCATION SECTION
        [HttpPost("CreateAutoBillingNotification")]
        public async Task<ActionResult<Autobillingnotification>> CreateAutoBillingNotification(AutoBillingNotificationModel autoBillingNotificationModel)
        {
            Autobillingnotification response = new Autobillingnotification();
            try
            {
                response = await _autoBillingNotificationService.CreateAutoBillingNotification(autoBillingNotificationModel);
                if (response.AutoBillingNotificationId == 0)
                {
                    return BadRequest(new { message = "Failed to create Staff User" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);


        }
        [HttpPost("UpdateAutoBillingNotification")]
        public async Task<ActionResult<Autobillingdraft>> UpdateAutoBillingNotification(AutoBillingNotificationModel autoBillingNotificationModel)
        {
            bool response = false;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse(406, "Invalid information."));
            }

            try
            {
                response = await _autoBillingNotificationService.UpdateAutoBillingNotification(autoBillingNotificationModel);
                if (!response)
                {
                    return BadRequest(new ApiResponse(304, "Record could not be updated."));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(500, "Internal Server error."));
            }

            return Ok(response);
        }
        [HttpGet("GetAutoBillingDocuments")]
        public async Task<ActionResult<List<BillingDocumentModel>>> GetAutoBillingDocuments()
        {
            var billingDocuments = await _billingDocumentsService.GetAllBillingDocumentDetails();
            return Ok(billingDocuments);
        }
        
        [HttpGet("GetAutoBillingNotificationById")]
        public async Task<ActionResult<Autobillingnotification>> GetAutoBillingNotificationById(int autoBillingNotificationId)
        {
            var billingnotification = await _autoBillingNotificationService.GetAutoBillingNotificationById(autoBillingNotificationId);
            return Ok(billingnotification);
        }
        [HttpGet("GetAllAutoBillingNotifications")]
        public async Task<ActionResult<Autobillingnotification>> GetAllAutoBillingNotifications()
        {
            var billingnotification = await _autoBillingNotificationService.GetAllAutoBillingNotifications();
            return Ok(billingnotification);
        }
        [HttpGet("GetAutoBillingNotificationByABPDId")]
        public async Task<ActionResult<Autobillingnotification>> GetAutoBillingNotificationByABPDId(int abpdId)
        {
            var billingnotification = await _autoBillingNotificationService.GetAutoBillingNotificationByABPDId(abpdId);
            return Ok(billingnotification);
        }
        [HttpGet("GetAutoBillingNotificationsByBillingType")]
        public async Task<ActionResult<Autobillingnotification>> GetAutoBillingNotificationsByBillingType(string billingType)
        {
            var billingnotification = await _autoBillingNotificationService.GetAutoBillingNotificationsByBillingType(billingType);
            return Ok(billingnotification);
        }
        [HttpGet("GetAutoBillingNotificationsByInvoiceType")]
        public async Task<ActionResult<Autobillingnotification>> GetAutoBillingNotificationsByInvoiceType(string invoiceType)
        {
            var billingnotification = await _autoBillingNotificationService.GetAutoBillingNotificationsByInvoiceType(invoiceType);
            return Ok(billingnotification);
        }
        [HttpGet("GetAutoBillingNotificationsByNotificationType")]
        public async Task<ActionResult<Autobillingnotification>> GetAutoBillingNotificationsByNotificationType(string notificationType)
        {
            var billingnotification = await _autoBillingNotificationService.GetAutoBillingNotificationsByNotificationType(notificationType);
            return Ok(billingnotification);
        }
        [HttpGet("GetAutoBillingNotificationsByNotificationDate")]
        public async Task<ActionResult<Autobillingnotification>> GetAutoBillingNotificationsByNotificationDate(DateTime notificationDate)
        {
            var billingnotification = await _autoBillingNotificationService.GetAutoBillingNotificationsByNotificationDate(notificationDate);
            return Ok(billingnotification);
        }

        [HttpGet("RegenrateAutobillingDraft")]
        public async Task<ActionResult<bool>> RegenrateAutobillingDraft(int billingDocumentId)
        {
            var result = await _autoBillingService.RegenrateAutoBillingDraft(billingDocumentId);
            return Ok(result);
        }
        // End Auto Billing NotifiCATION SECTION
    }
}
