using AutoMapper;
using Max.Core;
using Max.Core.Helpers;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantUnitOfWork _tenantUnitOfWork;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStaffUserService _staffUserService;
        private readonly ICookieService _cookieService;
        private readonly IReceiptHeaderService _receiptHeaderService;
        private readonly IInvoiceService _invoiceService;
        private readonly IEntityService _entityService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly ITenantService _tenantService;
        private readonly IBillingService _billingService;
        public AuthenticationService(IUnitOfWork unitOfWork,
                                     ITenantUnitOfWork _tenantUnitOfWork,
                                     IShoppingCartService shoppingCartService,
                                     IEmailService emailService,
                                     INotificationService notificationService,
                                     IHttpContextAccessor httpContextAccessor,
                                     IStaffUserService staffUserService,
                                     ICookieService cookieService,
                                     IReceiptHeaderService receiptHeaderService,
                                     IInvoiceService invoiceService,
                                     IEntityService entityService,
                                     IMapper mapper,
                                     ILogger<AuthenticationService> logger,
                                     ITenantService tenantService,
                                     IBillingService billingService)
        {
            this._unitOfWork = unitOfWork;
            this._tenantUnitOfWork = _tenantUnitOfWork;
            this._shoppingCartService = shoppingCartService;
            this._emailService = emailService;
            this._httpContextAccessor = httpContextAccessor;
            this._notificationService = notificationService;
            this._staffUserService = staffUserService;
            this._cookieService = cookieService;
            this._receiptHeaderService = receiptHeaderService;
            this._invoiceService = invoiceService;
            this._entityService = entityService;
            this._mapper = mapper;
            this._logger = logger;
            _tenantService = tenantService;
            _billingService = billingService;
        }

        public async Task<AutorizationResponseModel> Authenticate(AuthRequestModel authModel)
        {
            AutorizationResponseModel response = new AutorizationResponseModel();
            response.VerificationRequired = true;

            var user = await _unitOfWork.Staffusers.GetStaffUserByUserNameAsync(authModel.UserName);

            //Record accessor details
            authModel.UserName = authModel.UserName;
            authModel.IpAddress = authModel.IpAddress;
            authModel.Portal = (int)Portal.StaffPortal;
            var accessLog = await CreateAccessLog(authModel);

            // return null if user not found
            if (user == null)
            {
                response.ResponseStatus = new ResponseStatusModel();
                response.ResponseStatus.Message = "User name or password is incorrect.";
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }
            if (user.Locked == (int)UserAccountStatus.Locked)
            {
                response.ResponseStatus = new ResponseStatusModel();
                response.ResponseStatus.Message = $"Your account has been locked due to multiple incorrect login attempts. Please contact support team.";
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }

            byte[] salt = SecurePassword.HashHexStringToBytes(user.Salt);
            byte[] password = SecurePassword.HashHexStringToBytes(user.Password);

            PasswordHash hash = new PasswordHash();

            var isValidPassword = hash.IsValidPassword(user.Salt, user.Password, authModel.Password);

            if (!isValidPassword)
            {
                var failedAttempts = await _staffUserService.UpdateLoginStatus(user.UserId, (int)LoginStatus.Failed);
                response.ResponseStatus = new ResponseStatusModel();
                response.ResponseStatus.Message = "User name or password is incorrect.";
                if ((Constants.MAX_FAILED_ATTEMPTS - failedAttempts) > 0)
                {
                    response.ResponseStatus.Message += $" You have only {Constants.MAX_FAILED_ATTEMPTS - failedAttempts} login attempts left.";
                }
                else
                {
                    response.ResponseStatus.Message = $"Your account has been locked due to multiple incorrect login attempts. Please contact support team.";
                }
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }

            //Create device log
            UserDeviceModel deviceModel = new UserDeviceModel();
            deviceModel.UserId = user.UserId;
            deviceModel.Ipaddress = accessLog.IpAddress;
            deviceModel.DeviceName = accessLog.UserAgent;

            var device = await CreateDeviceLog(deviceModel);
            if (device.Authenticated == (int)LoginStatus.Success)
            {
                if (device.RemberDevice == (int)Status.Active)
                {
                    //Also check if last validation has expired

                    var lastValidation = (DateTime)device.LastValidated;
                    var validationExpiredCheck = lastValidation.AddDays(1 * Constants.DEVICE_VALIDATION_LIMIT);
                    if (DateTime.Now < validationExpiredCheck)
                    {
                        AutorizationResponseModel responseModel = new AutorizationResponseModel();

                        responseModel.UserId = user.UserId;
                        responseModel.VerificationRequired = false;
                        responseModel.VerificationToken = string.Empty;
                        responseModel.ResponseStatus = new ResponseStatusModel();
                        responseModel.ResponseStatus.Message = "Login device is alrady validated.";
                        responseModel.ResponseStatus.StatusCode = System.Net.HttpStatusCode.OK;

                        return (responseModel);
                    }
                }
            }

            var codeModel = new Multifactorcode();

            codeModel.UserId = user.UserId;
            codeModel.CreatDate = DateTime.UtcNow;
            codeModel.AccessCode = GenerateAccessToken();
            codeModel.ExpiryDate = DateTime.UtcNow.AddMinutes(authModel.VerificationMinutes);
            codeModel.DeviceId = device.UserDeviceId;
            codeModel.IpAddress = accessLog.IpAddress;

            await _unitOfWork.MultiFactorCodes.AddAsync(codeModel);
            await _unitOfWork.CommitAsync();

            AutorizationResponseModel model = new AutorizationResponseModel();

            model.emailDisplay = Mask.Email(user.Email);
            model.PhoneDisplay = Mask.Phone(user.CellPhoneNumber);
            model.UserId = user.UserId;
            model.VerificationRequired = true;
            model.VerificationToken = codeModel.AccessCode;
            model.ResponseStatus = new ResponseStatusModel();
            model.ResponseStatus.Message = "Login Validated.";
            model.ResponseStatus.StatusCode = System.Net.HttpStatusCode.OK;
            model.TenantName = authModel.OrganizationName;
            model.VerificationTimeLimit = authModel.VerificationMinutes;
            return (model);
        }

        public async Task<AutorizationResponseModel> AuthenticateEntity(AuthRequestModel authModel)
        {
            try
            {
                AutorizationResponseModel response = new AutorizationResponseModel();
                var entity = await _unitOfWork.Entities.GetEntityByWebLoginNameAsync(authModel.UserName);

                authModel.Portal = (int)Portal.MemberPortal;
                var accessLog = await CreateAccessLog(authModel);

                if (entity == null)
                {
                    response.ResponseStatus = new ResponseStatusModel();
                    response.ResponseStatus.Message = "User name or password is incorrect.";
                    response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    return response;
                }

                AutorizationResponseModel model = new AutorizationResponseModel();
                if (entity.CompanyId > 0)
                {
                    var company = await _unitOfWork.Companies.GetByIdAsync(Convert.ToInt32(entity.CompanyId));
                    var companyModel = _mapper.Map<CompanyModel>(company);
                    model.emailDisplay = companyModel.Emails.GetPrimaryEmail();
                    model.PhoneDisplay = companyModel.Phones.GetPrimaryPhoneNumber();
                }
                else if (entity.PersonId > 0)
                {
                    var person = await _unitOfWork.Persons.GetByIdAsync(Convert.ToInt32(entity.PersonId));
                    var personModel = _mapper.Map<PersonModel>(person);
                    model.emailDisplay = personModel.Emails.GetPrimaryEmail();
                    model.PhoneDisplay = personModel.Phones.GetPrimaryPhoneNumber();
                }

                model.UserId = entity.EntityId;
                model.VerificationRequired = true;
                model.VerificationToken = GenerateAccessToken();
                model.ResponseStatus = new ResponseStatusModel();
                model.ResponseStatus.Message = "Login Validated.";
                model.ResponseStatus.StatusCode = System.Net.HttpStatusCode.OK;
                model.TenantName = authModel.OrganizationName;
                model.VerificationTimeLimit = authModel.VerificationMinutes;
                return (model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ResponseStatusModel> SendMultiFactorToken(MultiFactorRequestModel model)
        {
            ResponseStatusModel response = new ResponseStatusModel();
            Multifactorcode code = new Multifactorcode();
            string userCellPhoneNumber;
            string userEmail;

            if (model.EntityId > 0)
            {
                _logger.LogInformation("Entering Authentication Service -> SendMultiFactorToken -> Entity Id ->" + model.EntityId.ToString());

                code = await _unitOfWork.MultiFactorCodes.GetByEntityIdAsync(model.EntityId);
                Person person = await _unitOfWork.Persons.GetPersonByEntityIdAsync(model.EntityId);
                PersonModel personModel = _mapper.Map<PersonModel>(person);
                userEmail = personModel.Emails.GetPrimaryEmail();
                userCellPhoneNumber = personModel.Phones.GetPrimaryPhoneNumber();
            }
            else
            {
                code = await _unitOfWork.MultiFactorCodes.GetByUserIdAsync(model.UserId);
                var user = _unitOfWork.Staffusers.GetStaffUserById(model.UserId);
                userEmail = user.Email;
                userCellPhoneNumber = user.CellPhoneNumber;
            }

            if (code == null)
            {
                response.Message = "Invalid access code. Please try again.";
                response.StatusCode = System.Net.HttpStatusCode.Unauthorized;

                return response;
            }
            //check if code has not expired
            if (DateTime.Compare(code.ExpiryDate, DateTime.UtcNow) < 0)
            {
                response.Message = "Access code has expired. Please try again.";
                response.StatusCode = System.Net.HttpStatusCode.Unauthorized;

                return response;
            }
            // Get OTP code

            string OTP = RandomNumber().ToString();

            //Send OTP
            if (model.Mode.ToUpper() == "EMAIL")
            {
                try
                {
                    await _emailService.SendMultiFactorNotification(model.TenantName, userEmail, OTP);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }
            }
            if (model.Mode.ToUpper() == "PHONE")
            {
                string message = $"{OTP} is your one time verification code for login to Trilix";
                _notificationService.SendMultiFactorSMSNotification(model.TenantName, message, userCellPhoneNumber);
            }

            var tenant = await _tenantService.GetTenantByOrganizationName(model.TenantName);
            code.ExpiryDate = DateTime.UtcNow.AddMinutes(tenant.VerificationMinutes ?? 0);
            code.Otpcode = OTP;

            _unitOfWork.MultiFactorCodes.Update(code);
            await _unitOfWork.CommitAsync();

            response.Message = "OTP code has been send. Please verify";
            response.StatusCode = System.Net.HttpStatusCode.OK;

            _logger.LogInformation("Exiting Authentication Service -> SendMultiFactorToken -> Entity Id ->" + model.EntityId.ToString());

            return response;
        }

        public async Task<ResponseStatusModel> ValidateMultiFactorToken(MultiFactorRequestModel model)
        {
            ResponseStatusModel response = new ResponseStatusModel();
            Multifactorcode code = new Multifactorcode();
            if (model.EntityId > 0)
            {
                code = await _unitOfWork.MultiFactorCodes.GetByEntityIdAsync(model.EntityId);
            }
            else
            {
                code = await _unitOfWork.MultiFactorCodes.GetByUserIdAsync(model.UserId);
            }
            if (code == null)
            {
                response.Message = "Invalid access code. Please try again.";
                response.StatusCode = System.Net.HttpStatusCode.Unauthorized;

                return response;
            }
            //check if code has not expired
            if (DateTime.Compare(code.ExpiryDate, DateTime.UtcNow) < 0)
            {
                response.Message = "Access code has expired. Please try again.";
                response.StatusCode = System.Net.HttpStatusCode.Unauthorized;

                return response;
            }
            if (model.EntityId > 0)
            {
                if (code.Entity.AccountLocked == (int)UserAccountStatus.Locked)
                {
                    response.Message = "Your account has been locked due to multiple incorrect attempts. Please contact support team.";
                    response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    return response;
                }
            }

            // Validate OTP code
            code.Attempts += 1;
            if (code.Otpcode != model.VerificationCode || code.AccessCode != model.VerificationToken)
            {
                if (model.EntityId > 0)
                {
                    await _entityService.UpdateLoginStatus(code, (int)LoginStatus.Failed, (int)LoginStatus.Failed);
                }
                else
                {
                    await _staffUserService.UpdateLoginStatus(code, (int)LoginStatus.Failed, (int)LoginStatus.Failed);
                }
                response.Message = $"Invalid verification code.Only {Constants.MAX_FAILED_ATTEMPTS - code.Attempts} more attempts left.";
                response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                if (code.Attempts >= (int)Constants.MAX_FAILED_ATTEMPTS)
                {
                    response.Message = $"Your account has been locked due to multiple incorrect attempts. Please contact support team.";
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                }

                response.OtpValidationSecondsLeft = (code.ExpiryDate - DateTime.UtcNow).TotalSeconds;
                return response;
            }
            code.CodeExpired = 1;
            code.CodeUsed = 1;
            if (model.EntityId > 0)
            {
                await _entityService.UpdateLoginStatus(code, model.RememberDevice, (int)LoginStatus.Success);
            }
            else
            {
                await _staffUserService.UpdateLoginStatus(code, model.RememberDevice, (int)LoginStatus.Success);
            }

            response.Message = "Authorized";
            response.StatusCode = System.Net.HttpStatusCode.OK;

            return response;

        }

        public async Task<AuthResponseModel> SendAuthorizationData(int userId, string tenantName)
        {
            try
            {
                StaffUserModel userModel = new StaffUserModel();
                ShoppingCartModel cart = new ShoppingCartModel();
                var user = _unitOfWork.Staffusers.GetStaffUserById(userId);
                var staffRoles = await _unitOfWork.Staffroles.GetAllStaffRolesByStaffIdAsync(userId);
                var reportEmailSender = await _unitOfWork.Staffusers.GetStaffUserByUserNameAsync("BillingService");
                userModel.FirstName = user.FirstName;
                userModel.LastName = user.LastName;
                userModel.UserId = user.UserId;
                userModel.UserName = user.UserName;
                userModel.OrganizationId = user.OrganizationId;
                userModel.OrganizationName = user.Organization.Name;
                userModel.AccountName = !string.IsNullOrEmpty(user.Organization.AccountName) ? user.Organization.AccountName : "Company";
                userModel.IsBirthdayRequired = user.Organization.IsBirthdayRequired == 1 ? true : false;
                if (staffRoles.Any())
                {
                    var checkIsAdminUser = staffRoles.FirstOrDefault(s => s.RoleId == 11);
                    if (checkIsAdminUser != null)
                    {
                        userModel.IsAdmin = true;
                    }
                    else
                    {
                        userModel.IsAdmin = false;
                    }
                }
                //Check if a cart is active
                if (user.UserName != "memberportal")
                {
                    cart = await _shoppingCartService.GetShoppingCartByUserId(user.UserId);
                }

                if (cart != null)
                {
                    userModel.CartId = cart.ShoppingCartId;
                }
                else
                {
                    userModel.CartId = 0;
                }

                var code = await _unitOfWork.MultiFactorCodes.GetByUserIdAsync(userId);

                if (code != null)
                {
                    // authentication successful so generate jwt token
                    var token = GenerateJwtToken(userModel.UserId);
                    var refreshToken = GenerateRefreshToken(code.IpAddress);
                    refreshToken.UserId = userId;
                    refreshToken.Token = token;
                    await AddAccessToken(refreshToken);

                    //Get tenantId

                    var tenant = await _tenantService.GetTenantByOrganizationName(tenantName);
                    string tenantId = string.Empty;

                    if (tenant != null)
                    {
                        tenantId = tenant.TenantId;
                    }



                    _cookieService.SetCookie("refreshToken", refreshToken.RefreshToken, 24 * 60);
                    _cookieService.SetCookie("tenantId", tenantId, 24 * 60);
                    var notifications = await _billingService.GetBillingNotifications();
                    return new AuthResponseModel(userModel, token, refreshToken.RefreshToken, code.IpAddress, tenantId, tenant.ConnectionString, tenant.ReportConnectionString, reportEmailSender.Email, notifications);
                }
                else
                {
                    throw new InvalidOperationException("We are unable to verify your identity. Please contact System Administrator.");
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<AuthResponseModel> SendEntityAuthorizationData(int userId, string tenantName, string ipAddress)
        {
            try
            {
                StaffUserModel userModel = new StaffUserModel();
                ShoppingCartModel cart = new ShoppingCartModel();
                var entity = _unitOfWork.Entities.GetEntityById(userId);
                if (entity != null)
                {
                    if (entity.PersonId > 0)
                    {
                        var person = await _unitOfWork.Persons.GetByIdAsync(Convert.ToInt32(entity.PersonId));
                        if (person != null)
                        {
                            var organizationDetails = await _unitOfWork.Organizations.GetByIdAsync(Convert.ToInt32(person.OrganizationId));
                            var personModel = _mapper.Map<PersonModel>(person);
                            userModel.FirstName = personModel.FirstName;
                            userModel.LastName = personModel.LastName;
                            userModel.UserId = userId;
                            userModel.UserName = personModel.FirstName + " " + personModel.LastName;
                            userModel.OrganizationId = organizationDetails.OrganizationId;
                            userModel.OrganizationName = organizationDetails.Name;
                            userModel.AccountName = !string.IsNullOrEmpty(organizationDetails.AccountName) ? organizationDetails.AccountName : "Company";
                        }
                    }
                    else if (entity.CompanyId > 0)
                    {
                        var company = await _unitOfWork.Companies.GetByIdAsync(Convert.ToInt32(entity.CompanyId));
                        if (company != null)
                        {
                            var companyModel = _mapper.Map<CompanyModel>(company);
                            userModel.FirstName = companyModel.CompanyName;
                            userModel.UserId = userId;
                            userModel.UserName = companyModel.CompanyName;
                        }
                    }

                    // authentication successful so generate jwt token
                    var token = GenerateJwtToken(userModel.UserId);
                    var refreshToken = GenerateRefreshToken(ipAddress);
                    refreshToken.EntityId = userId;
                    refreshToken.Token = token;
                    await AddAccessToken(refreshToken);

                    //Get tenantId

                    var tenant = await _tenantService.GetTenantByOrganizationName(tenantName);
                    string tenantId = string.Empty;

                    if (tenant != null)
                    {
                        tenantId = tenant.TenantId;
                    }

                    _cookieService.SetCookie("refreshToken", refreshToken.RefreshToken, 24 * 60);
                    _cookieService.SetCookie("tenantId", tenantId, 24 * 60);
                    return new AuthResponseModel(userModel, token, refreshToken.RefreshToken, ipAddress, tenantId, tenant.ConnectionString, tenant.ReportConnectionString, string.Empty, null);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<AuthResponseModel> SendAuthorizationDataForMemberPortal(int entityId, string tenantName, string IpAddress)
        {
            _logger.LogInformation("Entering Authentication Service -> SendAuthorizationDataForMemberPortal -> Entity Id -> " + entityId.ToString());

            var staff = await _unitOfWork.Staffusers.GetStaffUserByUserNameAsync("memberportal");
            var reportEmailSender = await _unitOfWork.Staffusers.GetStaffUserByUserNameAsync("BillingService");
            StaffUserModel userModel = new StaffUserModel();
            var entity = await _unitOfWork.Entities.GetEntityByIdAsync(entityId);
            userModel.FirstName = entity.People.ToArray()[0].FirstName;
            userModel.LastName = entity.People.ToArray()[0].LastName;
            userModel.UserId = entity.EntityId;
            userModel.UserName = entity.WebLoginName;
            userModel.OrganizationId = entity.OrganizationId ?? 0;
            userModel.OrganizationName = tenantName;


            // authentication successful so generate jwt token
            var token = GenerateJwtToken(entityId);
            var refreshToken = GenerateRefreshToken(IpAddress);
            refreshToken.EntityId = entityId;
            refreshToken.Token = token;
            await AddAccessToken(refreshToken);

            //Get tenantId

            var tenant = await _tenantService.GetTenantByOrganizationName(tenantName);
            string tenantId = string.Empty;

            if (tenant != null)
            {
                tenantId = tenant.TenantId;
            }
            _cookieService.SetCookie("refreshToken", refreshToken.RefreshToken, 24 * 60);
            _cookieService.SetCookie("tenantId", tenantId, 24 * 60);
            var notifications = await _billingService.GetBillingNotifications();
            _logger.LogInformation("Exiting Authentication Service -> SendAuthorizationDataForMemberPortal -> Entity Id -> " + entityId.ToString());
            return new AuthResponseModel(userModel, token, refreshToken.RefreshToken, IpAddress, tenantId, tenant.ConnectionString, tenant.ReportConnectionString, reportEmailSender.Email, notifications);

        }

        public async Task<TokenResponseModel> RefreshToken(TokenRequestModel model)
        {
            _logger.LogInformation($"RefreshToken -> UserId -> {model.UserId} - {model.RefreshToken} IP ->{model.IpAddress}");
            var accessToken = await _unitOfWork.AccessTokens.GetAccesTokensByRequestAsync(model.UserId, model.RefreshToken, model.IpAddress);
           
            if (accessToken == null)
            {
                var tokenResponse = new TokenResponseModel();
                tokenResponse.UserId = model.UserId;
                tokenResponse.ResponseStatus = new ResponseStatusModel();
                tokenResponse.ResponseStatus.Message = "Invalid refresh Token";
                tokenResponse.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Forbidden;
                return tokenResponse;
            }
            var user = await _unitOfWork.Staffusers.GetByIdAsync(model.UserId);

            //Update Current token

            accessToken.Revoked = DateTime.UtcNow;
            accessToken.RevokedIp = model.IpAddress;

            _unitOfWork.AccessTokens.Update(accessToken);

            // authentication successful so generate jwt token
            var token = GenerateJwtToken(user.UserId);
            var refreshToken = GenerateRefreshToken(model.IpAddress);
            refreshToken.UserId = model.UserId;
            refreshToken.Token = token;
            await AddAccessToken(refreshToken);
            var notifications = await _billingService.GetBillingNotifications();
            var response = new TokenResponseModel();
            response.UserId = model.UserId;
            response.Token = token;
            response.RefreshToken = refreshToken.RefreshToken;
            response.ResponseStatus = new ResponseStatusModel();
            response.ResponseStatus.Message = "Token Refreshed";
            response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.OK;
            response.Notifications = notifications;
            return response;
        }

        public async Task<TokenResponseModel> RefreshMemberToken(TokenRequestModel model)
        {
            var accessToken = await _unitOfWork.AccessTokens.GetEntityAccesTokensByRequestAsync(model.EntityId, model.RefreshToken, model.IpAddress);

            if (accessToken == null)
            {
                var tokenResponse = new TokenResponseModel();
                tokenResponse.EntityId = model.EntityId;
                tokenResponse.ResponseStatus = new ResponseStatusModel();
                tokenResponse.ResponseStatus.Message = "Invalid refresh token";
                tokenResponse.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Forbidden;
                return tokenResponse;
            }
            var entity = await _unitOfWork.Entities.GetByIdAsync(model.EntityId);

            //Update Current token

            accessToken.Revoked = DateTime.UtcNow;
            accessToken.RevokedIp = model.IpAddress;

            _unitOfWork.AccessTokens.Update(accessToken);

            // authentication successful so generate jwt token
            var token = GenerateJwtToken(entity.EntityId);
            var refreshToken = GenerateRefreshToken(model.IpAddress);
            refreshToken.EntityId = model.EntityId;
            refreshToken.Token = token;
            await AddAccessToken(refreshToken);

            var response = new TokenResponseModel();
            response.UserId = model.UserId;
            response.Token = token;
            response.RefreshToken = refreshToken.RefreshToken;
            response.ResponseStatus = new ResponseStatusModel();
            response.ResponseStatus.Message = "Token Refreshed";
            response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.OK;

            return response;
        }

        public async Task<bool> RevokeToken(TokenRequestModel model)
        {
            var accessToken = await _unitOfWork.AccessTokens.GetAccesTokensByRequestAsync(model.UserId, model.RefreshToken, model.IpAddress);

            if (accessToken == null)
            {
                return false;
            }
            //Update Current token

            accessToken.Revoked = DateTime.UtcNow;
            accessToken.RevokedIp = model.IpAddress;

            _unitOfWork.AccessTokens.Update(accessToken);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> ValidateToken(TokenRequestModel model)
        {
            var accessToken = await _unitOfWork.AccessTokens.GetAccesTokensByRequestAsync(model.UserId, model.RefreshToken, model.IpAddress);

            if (accessToken == null)
            {
                return false;
            }
            //Check expiration
            if (accessToken.Expire < DateTime.UtcNow)
            {
                accessToken.Revoked = DateTime.UtcNow;
                accessToken.RevokedIp = model.IpAddress;

                _unitOfWork.AccessTokens.Update(accessToken);
                await _unitOfWork.CommitAsync();
                return false;
            }
            return true;
        }

        public async Task<SelfPaymentResponseModel> ValidatePaymentUrl(PaymentRequestModel model)
        {
            SelfPaymentResponseModel paymentResponse = new SelfPaymentResponseModel();
            var paymentToken = Base64UrlEncoder.Decode(model.PaymentToken);

            var billingEmail = await _unitOfWork.BillingEmails.GetBillingEmailByTokenAsync(paymentToken);
            if (billingEmail != null)
            {
                try
                {
                    //Check if invoice has already been paid
                    var invoicePayment = await _invoiceService.GetInvoicePaymentsByInvoiceId(billingEmail.InvoiceId);
                    var invoice = await _unitOfWork.Invoices.GetInvoicePrintDetailsByIdAsync(billingEmail.InvoiceId);
                    InvoiceModel invoiceModel = _mapper.Map<InvoiceModel>(invoice);
                    paymentResponse.Invoice = invoiceModel;
                    var billingAddress = await _entityService.GetBillingAddressByEntityId(invoice.BillableEntityId ?? 0);
                    //check if any payment has been made
                    if (invoicePayment.Sum(x => x.Balance) <= 0)
                    {
                        paymentResponse.ShoppingCart = new ShoppingCartModel();

                        var receipt = await _unitOfWork.ReceiptHeaders.GetReceiptDetailsById(invoicePayment.Select(x => x.ReceiptId).ToArray());
                        var organization = await _unitOfWork.Organizations.GetOrganizationByIdAsync(receipt.FirstOrDefault().OrganizationId ?? 0);
                        paymentResponse.Organization = _mapper.Map<OrganizationModel>(organization);
                        paymentResponse.PaymentStatus = (int)PaymentTransactionStatus.Approved;
                        paymentResponse.BillingAddress = billingAddress;
                        paymentResponse.Invoice.BillingAddress = billingAddress;
                        paymentResponse.Invoice.BalanceAmount = invoicePayment.Sum(x => x.Balance);
                        paymentResponse.Invoice.PaidAmount = paymentResponse.Invoice.Amount - paymentResponse.Invoice.BalanceAmount;
                        paymentResponse.ReceiptList = _mapper.Map<List<ReceiptModel>>(receipt);
                        foreach (var item in paymentResponse.ReceiptList)
                        {
                            item.TotalAmount = receipt.Where(f => f.Receiptid == item.Receiptid).FirstOrDefault().Receiptdetails.Sum(f => f.Amount);
                        }
                        return paymentResponse;
                    }

                    var shoppingCart = await _shoppingCartService.AddMemberPortalInvoiceToShoppingCart(billingEmail.InvoiceId);
                    if (shoppingCart != null)
                    {
                        paymentResponse = await _shoppingCartService.AddMemberPortalReceiptToShoppingCart(shoppingCart.ShoppingCartId, (int)Status.InActive);
                        paymentResponse.Invoice = invoiceModel;
                        paymentResponse.BillingAddress = billingAddress;
                        paymentResponse.Invoice.BillingAddress = billingAddress;
                        paymentResponse.Invoice.BalanceAmount = invoicePayment.Sum(x => x.Balance);
                        paymentResponse.Invoice.PaidAmount = paymentResponse.Invoice.Amount - paymentResponse.Invoice.BalanceAmount;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }
            }

            return paymentResponse;
        }

        public async Task<ResetPasswordModel> ValidateResetPasswordRequest(ResetPasswordRequestModel model)
        {
            ResetPasswordModel response = new ResetPasswordModel();

            // Check if email is valid & account is active

            var staffUser = await _unitOfWork.Staffusers.GetStaffUserByEmailAsync(model.Email);

            return response;
        }

        public async Task<SelfPaymentReceiptResponseModel> ValidatePaymentReceiptRequest(SelfPaymentReceiptModel model)
        {
            SelfPaymentReceiptResponseModel paymentReceiptResponse = new SelfPaymentReceiptResponseModel();

            var paymentToken = Base64UrlEncoder.Decode(model.PaymentToken);

            var billingEmail = await _unitOfWork.BillingEmails.GetBillingEmailByTokenAsync(paymentToken);

            if (billingEmail != null)
            {
                //Get Receipt
                var receipt = await _receiptHeaderService.GetReceiptDetailByCartId(model.CartId);

                try
                {
                    //Get Payment Response
                    var paymentTransactions = await _unitOfWork.PaymentTransactions.GetPaymentTransactionsByReceiptIdAsync(receipt.Receiptid);

                    //Get the approved transaction
                    var paymentTransaction = paymentTransactions.Where(x => x.Status == (int)PaymentTransactionStatus.Approved).FirstOrDefault();
                    paymentReceiptResponse.AuthorizationCode = paymentTransaction.AuthCode;
                    paymentReceiptResponse.PaymentResponse = paymentTransaction.ResponseCode != null ? paymentTransaction.ResponseCode : "";
                    paymentReceiptResponse.TransactionId = paymentTransaction.TransactionId;
                    paymentReceiptResponse.Receipt = receipt;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return paymentReceiptResponse;
        }

        private async Task<Accesslog> CreateAccessLog(AuthRequestModel authModel)
        {

            var accessLog = new Accesslog();
            accessLog.Portal = authModel.Portal;
            accessLog.UserName = string.IsNullOrEmpty(authModel.UserName) ? "" : authModel.UserName;
            accessLog.IpAddress = authModel.IpAddress;
            accessLog.UserAgent = GetUserOS();
            accessLog.Referrer = GetReferer();
            accessLog.AccessDate = DateTime.UtcNow;
            try
            {
                await _unitOfWork.AccessLogs.AddAsync(accessLog);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return accessLog;
        }

        private async Task<Userdevice> CreateDeviceLog(UserDeviceModel model)
        {
            var userDevice = new Userdevice();
            IEnumerable<Userdevice> devices = Enumerable.Empty<Userdevice>();

            //check if this device already exists

            devices = await _unitOfWork.UserDevices.GetUserDevicesByUserIdAsync(model.UserId);

            var existingDevice = devices.Where(x => x.DeviceName == model.DeviceName
                                                && x.Authenticated == (int)LoginStatus.Success
                                                && x.Locked == (int)Status.InActive)
                                 .FirstOrDefault();
            if (existingDevice == null)
            {
                userDevice.UserId = model.UserId;
                userDevice.Ipaddress = model.Ipaddress;
                userDevice.DeviceName = model.DeviceName;
                userDevice.LastAccessed = DateTime.UtcNow;
                userDevice.Locked = (int)UserAccountStatus.Open;

                try
                {
                    await _unitOfWork.UserDevices.AddAsync(userDevice);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                existingDevice.LastAccessed = DateTime.UtcNow;
                try
                {
                    _unitOfWork.UserDevices.Update(existingDevice);
                    await _unitOfWork.CommitAsync();
                    return existingDevice;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return userDevice;
        }

        private async Task<Userdevice> CreateDeviceLogForMemberPortal(UserDeviceModel model)
        {
            var userDevice = new Userdevice();
            IEnumerable<Userdevice> devices = Enumerable.Empty<Userdevice>();

            //check if this device already exists


            devices = await _unitOfWork.UserDevices.GetUserDevicesByEntityIdAsync(model.EntityId);

            var existingDevice = devices.Where(x => x.DeviceName == model.DeviceName
                                                && x.Authenticated == (int)LoginStatus.Success
                                                && x.Locked == (int)Status.InActive)
                                 .FirstOrDefault();

            if (existingDevice == null)
            {
                userDevice.UserId = model.UserId;
                userDevice.EntityId = model.EntityId;
                userDevice.Ipaddress = model.Ipaddress;
                userDevice.DeviceName = model.DeviceName;
                userDevice.LastAccessed = DateTime.UtcNow;
                userDevice.Locked = (int)UserAccountStatus.Open;

                try
                {
                    await _unitOfWork.UserDevices.AddAsync(userDevice);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                existingDevice.LastAccessed = DateTime.UtcNow;
                try
                {
                    _unitOfWork.UserDevices.Update(existingDevice);
                    await _unitOfWork.CommitAsync();
                    return existingDevice;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return userDevice;
        }

        private async void UpdateDeviceStatus(int deviceId)
        {
            var device = await _unitOfWork.UserDevices.GetByIdAsync(deviceId);

            if (device != null)
            {
                try
                {
                    device.Authenticated = (int)LoginStatus.Success;
                    _unitOfWork.UserDevices.Update(device);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }



        public Staffuser GetStaffUserById(int id)
        {
            return _unitOfWork.Staffusers.GetStaffUserById(id);
        }

        public Entity GetEntityById(int id)
        {
            return _unitOfWork.Entities.GetEntityById(id);
        }

        public async Task<Accesstoken> AddAccessToken(AccessTokenModel model)
        {
            var accessToken = new Accesstoken();

            accessToken.UserId = model.UserId;
            accessToken.EntityId = model.EntityId;
            accessToken.Create = model.Create;
            accessToken.Expire = model.Expire;
            accessToken.CreatedIp = model.CreatedIp;
            accessToken.Token = model.Token;
            accessToken.RefreshToken = model.RefreshToken;

            await _unitOfWork.AccessTokens.AddAsync(accessToken);
            await _unitOfWork.CommitAsync();

            return accessToken;
        }


        private string GenerateJwtToken(int userId)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Secret-Key  Should be Moved to Database for better secuirty.");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userId.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(Constants.TOKEN_VALID_TIME),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateAccessToken()
        {

            var accessToken = TimeSensitivePassCode.GeneratePresharedKey();
            return accessToken;
        }

        private AccessTokenModel GenerateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new AccessTokenModel
                {
                    RefreshToken = Convert.ToBase64String(randomBytes),
                    Expire = DateTime.UtcNow.AddMinutes(30),
                    Create = DateTime.UtcNow,
                    CreatedIp = ipAddress
                };
            }
        }
        private readonly Random _random = new Random();

        // Generates a random number within a range.      
        public int RandomNumber()
        {
            int min = 100000;
            int max = 999999;
            return _random.Next(min, max);
        }

        public string GetUserOS()
        {
            string userOs = "unknown";
            try
            {
                userOs = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while reading client OS");
            }
            return userOs;
        }
        public string GetReferer()
        {
            string referer = "unknown";
            try
            {
                referer = _httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while reading referer");
            }
            return referer;
        }

        public async Task<MemberLoginResponseModel> AuthenticateMember(MemberLoginRequestModel memberLoginRequestModel)
        {
            MemberLoginResponseModel response = new MemberLoginResponseModel();

            _logger.LogInformation($"MemberPoprtal Auth Request Organization Name:{memberLoginRequestModel.OrganizationName} User Name:{memberLoginRequestModel.Username} Password:{memberLoginRequestModel.Password}");
            var entity = await _unitOfWork.Entities.GetEntityByUserNameAsync(memberLoginRequestModel.Username);

            //Record accessor details
            AuthRequestModel authModel = new AuthRequestModel();
            authModel.UserName = memberLoginRequestModel.Username;
            authModel.IpAddress = memberLoginRequestModel.IpAddress;
            authModel.Portal = (int)Portal.MemberPortal;

            var accessLog = await CreateAccessLog(authModel);

            if (entity == null)
            {
                response.ResponseStatus = new ResponseStatusModel();
                response.ResponseStatus.Message = "User name or password is incorrect.";
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }
            else
            {
                if (entity.AccountLocked == (int)UserAccountStatus.Locked && Constants.MAX_FAILED_ATTEMPTS == entity.PasswordFailedAttempts)
                {
                    response.ResponseStatus = new ResponseStatusModel();
                    response.ResponseStatus.Message = $"Your account has been locked due to multiple incorrect login attempts. Please contact support team.";
                    response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    _logger.LogInformation($"MemberPoprtal Auth Request Organization Name:{memberLoginRequestModel.OrganizationName} User Name:{memberLoginRequestModel.Username} Account Status:{UserAccountStatus.Locked} Failed Attempts: {entity.PasswordFailedAttempts}");
                    return response;
                }
                else if (entity.AccountLocked == (int)UserAccountStatus.Locked)
                {
                    response.ResponseStatus = new ResponseStatusModel();
                    response.ResponseStatus.Message = $"Your account has been locked. Please contact support team.";
                    response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    _logger.LogInformation($"MemberPoprtal Auth Request Organization Name:{memberLoginRequestModel.OrganizationName} User Name:{memberLoginRequestModel.Username} Account Status:{UserAccountStatus.Locked}");
                    return response;
                }
            }

            byte[] salt = SecurePassword.HashHexStringToBytes(entity.WebPasswordSalt);
            byte[] password = SecurePassword.HashHexStringToBytes(entity.WebPassword);
            _logger.LogInformation($"Web Password:{entity.WebPassword}");

            PasswordHash hash = new PasswordHash();

            var isValidPassword = hash.IsValidPassword(entity.WebPasswordSalt, entity.WebPassword, memberLoginRequestModel.Password);

            if (!isValidPassword)
            {
                var failedAttempts = await _entityService.UpdateLoginStatus(entity.EntityId, (int)LoginStatus.Failed);
                response.ResponseStatus = new ResponseStatusModel();
                response.ResponseStatus.Message = "User name or password is incorrect.";
                response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Unauthorized;

                if ((Constants.MAX_FAILED_ATTEMPTS - failedAttempts) > 0)
                {
                    response.ResponseStatus.Message += $" You have only {Constants.MAX_FAILED_ATTEMPTS - failedAttempts} login attempts left.";
                    response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                }
                else
                {
                    response.ResponseStatus.Message = $"Your account has been locked due to multiple incorrect login attempts. Please contact support team.";
                    response.ResponseStatus.StatusCode = System.Net.HttpStatusCode.Forbidden;
                }
                return response;
            }

            var memberPortalUser = await _unitOfWork.Staffusers.GetStaffUserByUserNameAsync("MemberPortal");

            //Create device log
            UserDeviceModel deviceModel = new UserDeviceModel();
            deviceModel.UserId = memberPortalUser.UserId;
            deviceModel.EntityId = entity.EntityId;
            deviceModel.Ipaddress = accessLog.IpAddress;
            deviceModel.DeviceName = accessLog.UserAgent;

            var device = await CreateDeviceLogForMemberPortal(deviceModel);

            if (device.Authenticated == (int)LoginStatus.Success)
            {
                if (device.RemberDevice == (int)Status.Active)
                {
                    //Also check if last validation has expired

                    var lastValidation = (DateTime)device.LastValidated;
                    var validationExpiredCheck = lastValidation.AddDays(1 * Constants.DEVICE_VALIDATION_LIMIT);
                    if (DateTime.Now < validationExpiredCheck)
                    {
                        MemberLoginResponseModel responseModel = new MemberLoginResponseModel();

                        responseModel.EntityId = entity.EntityId;
                        responseModel.VerificationRequired = false;
                        responseModel.VerificationToken = string.Empty;
                        responseModel.ResponseStatus = new ResponseStatusModel();
                        responseModel.ResponseStatus.Message = "Login device is already validated.";
                        responseModel.ResponseStatus.StatusCode = System.Net.HttpStatusCode.OK;
                        responseModel.OrganizationId = entity.OrganizationId ?? 0;

                        return (responseModel);
                    }
                }
            }

            var codeModel = new Multifactorcode();

            codeModel.UserId = memberPortalUser.UserId;
            codeModel.EntityId = entity.EntityId;
            codeModel.CreatDate = DateTime.UtcNow;
            codeModel.AccessCode = GenerateAccessToken();
            codeModel.ExpiryDate = DateTime.UtcNow.AddMinutes(memberLoginRequestModel.VerificationMinutes);
            codeModel.DeviceId = device.UserDeviceId;
            codeModel.IpAddress = accessLog.IpAddress;

            await _unitOfWork.MultiFactorCodes.AddAsync(codeModel);
            await _unitOfWork.CommitAsync();

            MemberLoginResponseModel model = new MemberLoginResponseModel();
            Person person = await _unitOfWork.Persons.GetPersonByIdAsync(entity.PersonId ?? 0);
            PersonModel personModel = _mapper.Map<PersonModel>(person);

            model.Username = entity.WebLoginName;
            model.PersonId = entity.PersonId ?? 0;
            model.Name = entity.Name;
            model.OrganizationId = entity.OrganizationId ?? 0;
            model.EntityId = entity.EntityId;
            model.EmailDisplay = Mask.Email(personModel.Emails.GetPrimaryEmail());
            model.VerificationRequired = true;
            model.VerificationToken = codeModel.AccessCode;
            model.ResponseStatus = new ResponseStatusModel();
            model.ResponseStatus.Message = "Login Validated.";
            model.ResponseStatus.StatusCode = System.Net.HttpStatusCode.OK;
            return model;

        }
    }
}