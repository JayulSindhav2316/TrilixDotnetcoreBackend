using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Services.Interfaces;
using Max.Core.Models;
using AuthorizeNetCore.Models;
using System;
using Newtonsoft.Json;

namespace Max.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthNetController : ControllerBase
    {
        private readonly ILogger<AuthNetController> _logger;
        private readonly IAuthNetService _authnetService;
        private readonly IOrganizationService _organizationService;
        private readonly IPaymentProfileService _paymentProfileService;
        private readonly IPaymentProcessorService _paymentProcessorService;

        public AuthNetController(   ILogger<AuthNetController> logger, 
                                    IAuthNetService authnetService, 
                                    IPaymentProfileService paymentProfileService, 
                                    IPaymentProcessorService paymentProcessorService,
                                    IOrganizationService organizationService)
        {
            _logger = logger;
            _authnetService = authnetService;
            _paymentProfileService = paymentProfileService;
            _paymentProcessorService = paymentProcessorService;
            _organizationService = organizationService;
        }

        [HttpPost("ProcessPaymentProfile")]
        public async Task<ActionResult<AuthNetPaymentProfileResponseModel>> ProcessPaymentProfile([FromForm] AuthNetSecureDataModel model)
        {
            try 
            {
                _logger.LogInformation("Calling ProcessPaymentProfile");
                _logger.LogInformation("ProcessPaymentProfile request :" + JsonConvert.SerializeObject(model));
                var response = await _authnetService.ProcessPaymentProfile(model);
                _logger.LogInformation("ProcessPaymentProfile response :" + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("ProcessPaymentProfile:", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("ProcessMemberPortalPaymentProfile")]
        public async Task<ActionResult<AuthNetPaymentProfileResponseModel>> ProcessMemberPortalPaymentProfile( AuthNetSecureDataModel model)
        {
            _logger.LogInformation($"Processing Payment profile: EntityId: {model.EntityId} Data value:{model.DataValue} Payment Method: {model.PaymentMode}");
            try
            {
                _logger.LogInformation("ProcessMemberPortalPaymentProfile request :" + JsonConvert.SerializeObject(model));
                var response = await _authnetService.ProcessPaymentProfile(model);
                _logger.LogInformation("ProcessMemberPortalPaymentProfile response :" + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("ProcessPaymentProfile:", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetPaymentProfile")]
        public async Task<ActionResult<PaymentProfileModel>> GetPaymentProfile([FromQuery] AuthNetPaymentProfileRequestModel model)
        {
            try
            {
                _logger.LogInformation("Calling GetPaymentProfile");
                _logger.LogInformation("GetPaymentProfile request : " + JsonConvert.SerializeObject(model));
                var response = await _authnetService.GetPaymentProfile(model);
                _logger.LogInformation("GetPaymentProfile response : " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPaymentProfile:", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("DeletePaymentProfile")]
        public async Task<ActionResult<AuthNetPaymentProfileResponseModel>> DeletePaymentProfile([FromForm] AuthNetPaymentProfileRequestModel model)
        {
            try
            {
                _logger.LogInformation("Calling DeletePaymentProfile");
                _logger.LogInformation("DeletePaymentProfile request :" + JsonConvert.SerializeObject(model));
                var response = await _authnetService.DeletePaymentProfile(model);
                _logger.LogInformation("DeletePaymentProfile response :" + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("DeletePaymentProfile:", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("DeleteMemberPortalPaymentProfile")]
        public async Task<ActionResult<AuthNetPaymentProfileResponseModel>> DeleteMemberPortalPaymentProfile( AuthNetPaymentProfileRequestModel model)
        {
            try
            {
                _logger.LogInformation("Calling DeleteMemberPortalPaymentProfile");
                _logger.LogInformation("DeleteMemberPortalPaymentProfile request :" + JsonConvert.SerializeObject(model));
                var response = await _authnetService.DeletePaymentProfile(model);
                _logger.LogInformation("DeleteMemberPortalPaymentProfile response :" + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("DeletePaymentProfile:", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("SetPreferredPaymentProfile")]
        public async Task<ActionResult<AuthNetPaymentProfileResponseModel>> SetPreferredPaymentProfile([FromForm] AuthNetPaymentProfileRequestModel model)
        {
            try
            {
                _logger.LogInformation("Calling SetPreferredPaymentProfile");
                _logger.LogInformation("SetPreferredPaymentProfile request :" + JsonConvert.SerializeObject(model));
                var response = await _paymentProfileService.SetPreferredPaymentMethod(model.EntityId, model.PaymentProfileId);
                _logger.LogInformation("SetPreferredPaymentProfile response :" + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("SetPrefferedPaymentProfile:", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("SetMemberPortalPreferredPaymentProfile")]
        public async Task<ActionResult<AuthNetPaymentProfileResponseModel>> SetMemberPortalPreferredPaymentProfile( AuthNetPaymentProfileRequestModel model)
        {
            _logger.LogInformation($"Updating preferred Payment profile: EntityId: {model.EntityId} Payment Profile Id:{model.PaymentProfileId}");
            try
            {
                _logger.LogInformation("SetMemberPortalPreferredPaymentProfile request :" + JsonConvert.SerializeObject(model));
                var response = await _paymentProfileService.SetPreferredPaymentMethod(model.EntityId, model.PaymentProfileId);
                _logger.LogInformation("SetMemberPortalPreferredPaymentProfile response :" + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("SetPrefferedPaymentProfile:", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("SetAutoBillingPaymentProfile")]
        public async Task<ActionResult<AuthNetPaymentProfileResponseModel>> SetAutoBillingPaymentProfile([FromForm] AuthNetPaymentProfileRequestModel model)
        {
            try
            {
                _logger.LogInformation("Calling SetAutoBillingPaymentProfile");
                _logger.LogInformation("SetAutoBillingPaymentProfile request :" + JsonConvert.SerializeObject(model));
                var response = await _paymentProfileService.SetAutoBillingPaymentMethod(model.EntityId, model.PaymentProfileId);
                _logger.LogInformation("SetAutoBillingPaymentProfile response :" + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("SetAutoBillingPaymentProfile:", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetPaymentProcessorInfo")]
        public async Task<ActionResult<MerchantConfigModel>> GetPaymentProcessorInfo(string organizationName)
        {
            try
            {
                _logger.LogInformation("Calling GetPaymentProcessorInfo");
                _logger.LogInformation("OrganizationName :" + organizationName);
                var organization = await _organizationService.GetOrganizationByName(organizationName);

                if(organization != null)
                {
                    var processorConfig = await _paymentProcessorService.GetPaymentProcessorInfoByOrganizationId(organization.OrganizationId);
                    if (processorConfig == null)
                    {
                        _logger.LogInformation("GetPaymentProcessorInfo response : The Organization name is invalid");
                        return BadRequest(new { message = "The Organization name is invalid." });
                    }
                    _logger.LogInformation("GetPaymentProcessorInfo response :" + JsonConvert.SerializeObject(processorConfig));
                    return Ok(processorConfig);
                }
                _logger.LogInformation("GetPaymentProcessorInfo response : The Organization name is invalid");
                return BadRequest(new { message = "The Organization name is invalid." });
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPaymentProcessorInfo:", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}