using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Max.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly ITenantService _tenantService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IAuthNetService _authNetService;
        private readonly IResetPasswordService _resetPasswordService;
        private readonly IPersonService _personService;
        private readonly IEmailService _emailService;
        private readonly IContactTokenService _contactTokenService;

        public AuthorizationController(ILogger<RoleController> logger, 
                                        IAuthenticationService authenticationService, 
                                        ITenantService tenantService,
                                        IShoppingCartService shoppingCartService, 
                                        IAuthNetService authNetService,
                                        IResetPasswordService resetPasswordService,
                                        IPersonService personService,
                                        IEmailService emailService,
                                        IContactTokenService contactTokenService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _tenantService = tenantService;
            _shoppingCartService = shoppingCartService;
            _authNetService = authNetService;
            _resetPasswordService = resetPasswordService;
            _personService = personService;
            _emailService = emailService;
            _contactTokenService =  contactTokenService;
        }

        [HttpPost("ValidateOrganization")]
        public async Task<ActionResult<Tenant>> ValidateOrganization(AuthRequestModel model)
        {
            //We are vaidating Organization first from the Tenant database

            var tenant = await _tenantService.GetTenantByOrganizationName(model.OrganizationName);
            if (tenant == null)
            {
                return BadRequest(new { message = "The Organization name is invalid." });
            }

            return Ok(tenant);
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AutorizationResponseModel>>Authenticate(AuthRequestModel model)
        {
            //We are vaidating Organization first from the Tenant database

            var tenant = await _tenantService.GetTenantByOrganizationName(model.OrganizationName);
            if (tenant == null)
            {
                return BadRequest(new { message = "The Organization name is invalid." });
            }

            model.VerificationMinutes = tenant.VerificationMinutes ?? 0;
            var response = await  _authenticationService.Authenticate(model);
            _logger.LogInformation($"Authenticate Response: User Id {response.UserId} Status:{response.ResponseStatus} Verification Required: {response.VerificationRequired}");
            if (response.ResponseStatus.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return BadRequest(new { message = response.ResponseStatus.Message });
            }
            response.VerificationRequired = false;
            if (!response.VerificationRequired)
            {
                var authorization = await _authenticationService.SendAuthorizationData(response.UserId, model.OrganizationName);
                _logger.LogInformation($"Authenticate->SendAuthorizationData: User Name:{authorization.Username } Organization Name:{authorization.OrganizationName} Tenant Id:{authorization.TenantId} Token:{authorization.Token} Refresh Token:{authorization.RefreshToken}");

                return Ok(authorization);
            }

            return Ok(response);
        }
        [HttpPost("GetMultiFactorCode")]
        public async Task<ActionResult<ResponseStatusModel>> GetMultiFactorCode(MultiFactorRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "An error has occured while sending verification data." });
            }
            var response = await _authenticationService.SendMultiFactorToken(model);

            if (response == null)
                return BadRequest(new { message = "An error has occured while sending verification data." });

            return response;
        }
        [HttpPost("ValidateMultiFactorCode")]
        public async Task<ActionResult<ResponseStatusModel>> ValidateMultiFactorCode(MultiFactorRequestModel model)
        {
            var response = await _authenticationService.ValidateMultiFactorToken(model);
            _logger.LogInformation($"ValidateMultiFactorCode: {response.Message } :{response.StatusCode}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK && model.EntityId == 0)
            {
                var authorization = await _authenticationService.SendAuthorizationData(model.UserId, model.TenantName);
                _logger.LogInformation($"SendAuthorizationData: User Name:{authorization.Username } Organization Name:{authorization.OrganizationName} Tenant Id:{authorization.TenantId} Token:{authorization.Token} Refresh Token:{authorization.RefreshToken}");
                return Ok(authorization);
            }
            
            if (response == null)
                return BadRequest(new { message = "An error has occured while sending verification data." });

            return response;
        }
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<TokenResponseModel>> RefreshToken(TokenRequestModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new { message = "An error has occured while sending verification data." });
            }
            var response = await _authenticationService.RefreshToken(model);

            if(response.ResponseStatus.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return Forbid();
            }
            if (response == null)
            {
                return BadRequest(new { message = "An error has occured while sending verification data." });
            }
            return response;
        }
        [HttpPost("RevokeToken")]
        public async Task<ActionResult<bool>> RevokeToken(TokenRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "An error has occured while sending verification data." });
            }
            var response = await _authenticationService.RevokeToken(model);
           
            if (!response)
            {
                return BadRequest(new { message = "An error has occured while sending verification data." });
            }
            return response;
        }
        [HttpPost("ValidateToken")]
        public async Task<ActionResult<bool>> ValidateToken(TokenRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "An error has occured while sending verification data." });
            }
            var response = await _authenticationService.ValidateToken(model);

            if (!response)
            {
                return BadRequest(new { message = "An error has occured while sending verification data." });
            }
            return response;
        }
        [HttpPost("ValidatePaymentUrl")]
        public async Task<ActionResult<SelfPaymentResponseModel>> ValidatePaymentUrl(PaymentRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "An error has occured while sending verification data." });
            }
            var response = await _authenticationService.ValidatePaymentUrl(model);

            return response;
        }
        [HttpPost("ProcessMemberPayment")]
        public async Task<ActionResult<ShoppingCartModel>> ProcessMemberPayment([FromForm] AuthNetSecureDataModel model)
        {
            if (!ModelState.IsValid)
            {
                String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception));
                _logger.LogError("Shoppingcart:", messages);
            }
            //Check if cart exists 
            try
            {
                _logger.LogInformation("Shoppingcart Id:", model.CartId);
                var cart = await _shoppingCartService.GetShoppingCartById(model.CartId);
                if (cart == null)
                {
                    _logger.LogError("Cart Not Found");
                    return BadRequest(new { message = "Cart not found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }
           
           
            try
            {
                var shoppingCart = await _authNetService.ProcessAcceptPayment(model);
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }
          
        }
        [HttpPost("GetSelfPaymentReceipt")]
        public async Task<ActionResult<SelfPaymentReceiptResponseModel>> GetSelfPaymentReceipt(SelfPaymentReceiptModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "An error has occured while sending verification data." });
            }
            var response = await _authenticationService.ValidatePaymentReceiptRequest(model);

            return response;
        }

        [HttpPost("AuthenticateAPIMemberPortal")]
        public async Task<ActionResult<MemberPortalAPIAuthResponseModel>> AuthenticateAPIMemberPortal(AuthRequestModel model)
        {
            
            //We are vaidating Organization first from the Tenant database

            var tenant = await _tenantService.GetTenantByOrganizationName(model.OrganizationName);
            if (tenant == null)
            {
                return BadRequest(new { message = "The Organization name is invalid." });
            }

            var response = await _authenticationService.AuthenticateEntity(model);
            if (response.ResponseStatus.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var authorization = await _authenticationService.SendEntityAuthorizationData(response.UserId, model.OrganizationName, model.IpAddress);
                try
                {
                    MemberPortalAPIAuthResponseModel memberPortalAPIAuthResponseModel = new MemberPortalAPIAuthResponseModel();
                    memberPortalAPIAuthResponseModel.ResponseStatus = response.ResponseStatus;
                    memberPortalAPIAuthResponseModel.Id = authorization.Id;
                    memberPortalAPIAuthResponseModel.FirstName = authorization.FirstName;
                    memberPortalAPIAuthResponseModel.LastName = authorization.LastName;
                    memberPortalAPIAuthResponseModel.Username = authorization.Username;
                    memberPortalAPIAuthResponseModel.Token = authorization.Token;
                    memberPortalAPIAuthResponseModel.TenantId = authorization.TenantId;
                    memberPortalAPIAuthResponseModel.OrganizationId = authorization.OrganizationId;
                    memberPortalAPIAuthResponseModel.TenantName = authorization.OrganizationName;
                    memberPortalAPIAuthResponseModel.TenantCN = authorization.TenantCN;
                    memberPortalAPIAuthResponseModel.TenantRCN = authorization.TenantRCN;
                    memberPortalAPIAuthResponseModel.RefreshToken = authorization.RefreshToken;
                    memberPortalAPIAuthResponseModel.IpAddress = authorization.IpAddress;
                    return Ok(memberPortalAPIAuthResponseModel);
                }
                catch(Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            return BadRequest(new { message = response.ResponseStatus.Message });
        }

        [HttpPost("ConfirmResetPassword")]
        public async Task<ActionResult<ResetPasswordModel>> ConfirmResetPassword(ResetPasswordRequestModel model)
        {

            //We are vaidating Organization first from the Tenant database

            var tenant = await _tenantService.GetTenantByOrganizationName(model.OrganizationName);
            if (tenant == null)
            {
                return BadRequest(new { message = "The Organization name is invalid." });
            }

            var response = await _resetPasswordService.CreateResetRequest(model);
            return Ok(response);
        }
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ResetPasswordModel>> ResetPassword(ResetPasswordRequestModel model)
        {

            //We are vaidating Organization first from the Tenant database

            var tenant = await _tenantService.GetTenantByOrganizationName(model.OrganizationName);
            if (tenant == null)
            {
                return BadRequest(new { message = "The Organization name is invalid." });
            }

            var response = await _resetPasswordService.ResetPassword(model);
            return Ok(response);
        }
        [HttpPost("ResetPasswordLink")]
        public async Task<ActionResult<ResetPasswordModel>> ResetPasswordLink(ResetPasswordRequestModel model)
        {

            //We are vaidating Organization first from the Tenant database

            var tenant = await _tenantService.GetTenantByOrganizationName(model.OrganizationName);
            if (tenant == null)
            {
                return BadRequest(new { message = "The Organization name is invalid." });
            }

            try
            {
                var response = await _resetPasswordService.ValidateResetPasswordLink(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }
        [HttpPost("ValidateEmail")]
        public async Task<ActionResult> ValidateEmail(MemberAccountEmailModel model)
        {
            if(model.EmailAddress == null || model.EmailAddress == "")
            {
                return BadRequest(new { message = "Email address is required." });
            }
            var result = await _personService.IsUniqueueEmail(model.EmailAddress);
            if(result == false)
            {
                //Send password Reset link
                try
                {
                    ResetPasswordRequestModel resetRequestModel = new ResetPasswordRequestModel();
                    resetRequestModel.Email = model.EmailAddress;
                    resetRequestModel.IpAddress = model.IpAddress;
                    resetRequestModel.OrganizationName= model.OrganizationName;
                    resetRequestModel.TenantName= model.OrganizationName;

                    var response = await _resetPasswordService.CreateMemberPasswordResetRequest(resetRequestModel);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"RequestResetPassword: Error: {ex.Message} {ex.StackTrace}");
                    var response = new ResetPasswordModel();
                    response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.UnprocessableEntity;
                    response.ResponseStatus.Message = ex.Message;
                    _logger.LogInformation($"Reset Password request: organization Name:{model.OrganizationName} {model.EmailAddress} Status:{response.ResponseStatus.StatusCode}  response message:{response.ResponseStatus.Message}");
                    return new ObjectResult(response) { StatusCode = 422 };
                }
            }
            else
            {
                //Send validation code through email
                try
                {
                    var contactToken = await _contactTokenService.CreateContactToken(model.EmailAddress, model.IpAddress);
                    if(contactToken!=null)
                    {
                        model.Token = contactToken.Token;
                    }
                    var emailSent = await _emailService.SendAccountVerificationEmailBody(model);
                    return Ok(contactToken);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message.ToString() });
                }

            }
        }
        [HttpPost("ValidateMemberPortalAccount")]
        public async Task<ActionResult<Person>> ValidateMemberPortalAccount(MemberPortalVerificationModel model)
        {
            _logger.LogInformation("Validation Data: First Name:{model.FirstName} Last Name:{model.LastName} Email:{model.EmailAddress} Phone Number:{model.PhoneNumber} BirhDate:{model.BirthDate}", model);

            if(!String.IsNullOrEmpty(model.BirthDate))
            {
                model.BirthDate.Replace(".", "/");
            }
            if (String.IsNullOrEmpty(model.FirstName) || String.IsNullOrEmpty(model.LastName) || String.IsNullOrEmpty(model.PhoneNumber))
            {
                return BadRequest(new { message = "Please provide first name, last name & phone number." });
            }
            try
            {
                var result = await _personService.IsUniqueueMemberPortalAccount(model);
                if (result)
                {
                    DateTime dateOfBirth = DateTime.Parse(model.BirthDate);
                    PersonModel person = new PersonModel();
                    person.FirstName = model.FirstName;
                    person.LastName = model.LastName;
                    person.DateOfBirth = dateOfBirth;

                    PhoneModel phone = new PhoneModel();
                    phone.PhoneType = "Other";
                    phone.PhoneNumber= model.PhoneNumber;
                    phone.IsPrimary = (int)Status.Active;
                    person.Phones.Add(phone);

                    EmailModel email = new EmailModel();
                    email.EmailAddressType = "Other";
                    email.EmailAddress = model.EmailAddress;
                    email.isPrimary = true; 
                    person.Emails.Add(email);
                    person.CompanyName = String.Empty;
                    var response = await _personService.CreatePerson(person);
                    return Ok(response);
                }
                else
                {
                    return BadRequest(new Person());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }
    }
}