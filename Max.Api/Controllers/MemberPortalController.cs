using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberPortalController : ControllerBase
    {
        private readonly ILogger<MemberPortalController> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPersonService _PersonService;
        private readonly IDocumentService _documentService;
        [System.Obsolete]
        private readonly IHostEnvironment _appEnvironment;
        private readonly IInvoiceService _invoiceService;
        private readonly IEntityService _entityService;
        private readonly IStaffUserService _staffUserService;
        private readonly ITenantService _tenantService;
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IResetPasswordService _resetPasswordService;

        [Obsolete]
        public MemberPortalController(ILogger<MemberPortalController> logger,
            IAuthenticationService authenticationService,
            IPersonService PersonService,
            IDocumentService DocumentService,
            IHostEnvironment appEnvironment,
            IInvoiceService invoiceService,
            IEntityService entityService,
            IStaffUserService staffUserService,
            ITenantService tenantService,
            IGroupService groupService,
            IGroupMemberService groupMemberService,
            IResetPasswordService resetPasswordService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _PersonService = PersonService;
            _documentService = DocumentService;
            _appEnvironment = appEnvironment;
            _invoiceService = invoiceService;
            _entityService = entityService;
            _staffUserService = staffUserService;
            _tenantService = tenantService;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _resetPasswordService = resetPasswordService;
        }

        [HttpPost("AuthenticateMember")]
        public async Task<ActionResult<MemberLoginResponseModel>> AuthenticateMember(MemberLoginRequestModel memberLoginRequestModel)
        {
            var response = new MemberLoginResponseModel();
            var tenant = await _tenantService.GetTenantByOrganizationName(memberLoginRequestModel.OrganizationName);
            if (tenant == null)
            {
                response = new MemberLoginResponseModel();
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.ResponseStatus.Message = "The Organization is invalid. Please contact support team.";
                return new ObjectResult(response) { StatusCode = 401 };
            }

            memberLoginRequestModel.VerificationMinutes = tenant.VerificationMinutes ?? 0;
            response = await _authenticationService.AuthenticateMember(memberLoginRequestModel);
            _logger.LogDebug($" Authenticate Resposne: Entity Id {response.EntityId} Status:{response.ResponseStatus.StatusCode} Verification Required: {response.VerificationRequired} response message:{response.ResponseStatus.Message}");
            if (response == null)
            {
                response = new MemberLoginResponseModel();
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                response.ResponseStatus.Message = "User name or password is incorrect.";
                _logger.LogDebug($" Authenticate Resposne: Entity Id {response.EntityId} Status:{response.ResponseStatus.StatusCode} Verification Required: {response.VerificationRequired} response message:{response.ResponseStatus.Message}");
                return new ObjectResult(response) { StatusCode = 401 };
            }

            if(response.ResponseStatus.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var authorization = await _authenticationService.SendAuthorizationDataForMemberPortal(response.EntityId, memberLoginRequestModel.OrganizationName, memberLoginRequestModel.IpAddress);
                response.AuthenticationToken = authorization.Token;
                response.RefreshToken = authorization.RefreshToken;
                response.Groups = await _groupMemberService.GetGroupsByEntityId(response.EntityId);
                _logger.LogDebug($" Authenticate Resposne: Entity Id {response.EntityId} Status:{response.ResponseStatus.StatusCode} Verification Required: {response.VerificationRequired} response message:{response.ResponseStatus.Message}");
                return Ok(response);
            }
            else
            {
                _logger.LogDebug($" Authenticate Resposne: Entity Id {response.EntityId} Status:{response.ResponseStatus.StatusCode} Verification Required: {response.VerificationRequired} response message:{response.ResponseStatus.Message}");
                return new ObjectResult(response) { StatusCode = 401 };
            }
           
        }

        [HttpPost("AuthenticateMemberForTrilix")]
        public async Task<ActionResult<MemberLoginResponseModel>> AuthenticateMemberForTrilix(MemberLoginRequestModel memberLoginRequestModel)
        {
            var response = new MemberLoginResponseModel();
            var tenant = await _tenantService.GetTenantByOrganizationName(memberLoginRequestModel.OrganizationName);
            if (tenant == null)
            {
                response = new MemberLoginResponseModel();
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.ResponseStatus.Message = "The Organization is invalid. Please contact support team.";
                _logger.LogDebug($" Authenticate Resposne: Entity Id {response.EntityId} Status:{response.ResponseStatus.StatusCode} Verification Required: {response.VerificationRequired} response message:{response.ResponseStatus.Message}");
                return new ObjectResult(response) { StatusCode = 422 };
            }

            memberLoginRequestModel.VerificationMinutes = tenant.VerificationMinutes ?? 0;
            response = await _authenticationService.AuthenticateMember(memberLoginRequestModel);
            if (response == null)
            {
                response = new MemberLoginResponseModel();
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.ResponseStatus.Message = "User name or password is incorrect.";
                _logger.LogDebug($" Authenticate Resposne: Entity Id {response.EntityId} Status:{response.ResponseStatus.StatusCode} Verification Required: {response.VerificationRequired} response message:{response.ResponseStatus.Message}");
                return new ObjectResult(response) { StatusCode = 403 };
            }
            _logger.LogDebug($" Authenticate Resposne: Entity Id {response.EntityId} Status:{response.ResponseStatus.StatusCode} Verification Required: {response.VerificationRequired} response message:{response.ResponseStatus.Message}");

            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetPersonProfileByEntityId")]
        public async Task<ActionResult<EntityModel>> GetPersonProfileByEntityId(int entityId)
        {
            if (entityId < 0) return BadRequest(new { message = "Invalid Person Id." });
            var entityModel = new EntityModel();
            try
            {
                entityModel = await _entityService.GetEntityProfileById(entityId);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(entityModel);
        }

        [Authorize]
        [HttpGet("GetPersonById")]
        public async Task<ActionResult<PersonProfileModel>> GetPersonById(int personId)
        {
            if (personId < 0) return BadRequest(new { message = "Invalid Person Id." });
            var person = new PersonModel();
            try
            {
                person = await _PersonService.GetPersonById(personId);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(person);
        }

        [Authorize]
        [HttpGet("GetAllPersonsByMembershipId")]
        public async Task<ActionResult<IEnumerable<PersonModel>>> GetAllPersonsByMembershipId(int membershipId)
        {
            var personModel = await _PersonService.GetAllPersonsByMembershipId(membershipId);
            return Ok(personModel);
        }


        [Authorize]
        [HttpGet("GetInvoicesByInvoiceIds")]
        public async Task<ActionResult<IEnumerable<InvoiceModel>>> GetInvoicesByInvoiceIds(string invoiceIds)
        {
            List<InvoicePaymentModel> invoicePaymentModel = new List<InvoicePaymentModel>();
            List<InvoicePaymentModel> lstInvoices = new List<InvoicePaymentModel>();

            try
            {
                if (invoiceIds.Length > 0)
                {
                    string[] arrInvoiceIds = invoiceIds.Split(',');

                    foreach (var invoice in arrInvoiceIds)
                    {
                        invoicePaymentModel = await _invoiceService.GetInvoicePaymentsByInvoiceId(Convert.ToInt32(invoice));

                        if (invoicePaymentModel != null)
                        {
                            lstInvoices.AddRange(invoicePaymentModel);
                        }
                    }
                }
                else
                {
                    return BadRequest(new { message = "No Invoice Ids found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(lstInvoices);
        }
        [HttpPost("RequestResetPassword")]
        public async Task<ActionResult<ResetPasswordModel>> RequestResetPassword(ResetPasswordRequestModel model)
        {
            //We are vaidating Organization first from the Tenant database
            _logger.LogInformation($"Reset Password request: organization Name:{model.OrganizationName} {model.Email}");
            var tenant = await _tenantService.GetTenantByOrganizationName(model.OrganizationName);
            if (tenant == null)
            {
                var response = new ResetPasswordModel();
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.ResponseStatus.Message = "The Organization is invalid. Please contact support team.";
                _logger.LogInformation($"Reset Password request: organization Name:{model.OrganizationName} {model.Email} Status:{response.ResponseStatus.StatusCode}  response message:{response.ResponseStatus.Message}");
                return new ObjectResult(response) { StatusCode = 422 };
            }

            try
            {
                var response = await _resetPasswordService.CreateMemberPasswordResetRequest(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"RequestResetPassword: Error: {ex.Message} {ex.StackTrace}");
                var response = new ResetPasswordModel();
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.UnprocessableEntity;
                response.ResponseStatus.Message = ex.Message;
                _logger.LogInformation($"Reset Password request: organization Name:{model.OrganizationName} {model.Email} Status:{response.ResponseStatus.StatusCode}  response message:{response.ResponseStatus.Message}");
                return new ObjectResult(response) { StatusCode = 422 };
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ResetPasswordModel>> ResetPassword(ResetPasswordRequestModel model)
        {

            //We are vaidating Organization first from the Tenant database
            _logger.LogInformation($"Reset Password request: organization Name:{model.OrganizationName} {model.Email} Password: {model.Password}");
            var tenant = await _tenantService.GetTenantByOrganizationName(model.OrganizationName);
            if (tenant == null)
            {
                var response = new ResetPasswordModel();
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.ResponseStatus.Message = "The Organization is invalid. Please contact support team.";
                _logger.LogInformation($"Reset Password : organization Name:{model.OrganizationName} {model.Email} Status:{response.ResponseStatus.StatusCode}  response message:{response.ResponseStatus.Message}");
                return new ObjectResult(response) { StatusCode = 422 };
            }

            try
            {
                var response = await _resetPasswordService.ResetMemberPassword(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ResetPassword: Error: {ex.Message}");
                var response = new ResetPasswordModel();
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.UnprocessableEntity;
                response.ResponseStatus.Message = ex.Message;
                _logger.LogInformation($"Reset Password : organization Name:{model.OrganizationName} {model.Email} Status:{response.ResponseStatus.StatusCode}  response message:{response.ResponseStatus.Message}");
                return new ObjectResult(response) { StatusCode = 422 };
            }
        }
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<TokenResponseModel>> RefreshToken(TokenRequestModel model)
        {
            var response = new TokenResponseModel();
            if (!ModelState.IsValid)
            {
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.UnprocessableEntity;
                response.ResponseStatus.Message = "Invalid Token request.";
                return new ObjectResult(response) { StatusCode = 422 };
            }
            _logger.LogInformation($"RefreshToken request: EntityId:{model.EntityId} IpAddress:{model.IpAddress} RefreshToken: {model.RefreshToken} UserId: {model.UserId}");
            response = await _authenticationService.RefreshMemberToken(model);

            if (response.ResponseStatus.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                response.ResponseStatus.Message = "Invalid Token request.";
                return new ObjectResult(response) { StatusCode = 403 };
            }
            if (response == null)
            {
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.UnprocessableEntity;
                response.ResponseStatus.Message = "Invalid Token request.";
                return new ObjectResult(response) { StatusCode = 422 };
            }
            return response;
        }
    }
}
