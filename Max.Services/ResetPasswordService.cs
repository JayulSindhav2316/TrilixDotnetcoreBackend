using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthNetService _authNetService;
        private readonly IStaffUserService _staffUserService;
        private readonly IEmailService _emailService;
        private readonly ITenantService _tenantService;
        public ResetPasswordService(IUnitOfWork unitOfWork,
                                IMapper mapper,
                                IEmailService emailService,
                                IAuthNetService authNetService,
                                IStaffUserService staffUserService,
                                ITenantService tenantService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._authNetService = authNetService;
            this._staffUserService = staffUserService;
            this._emailService = emailService;
            this._tenantService = tenantService;
        }

        public async Task<Resetpassword> GetResetRequestById(int id)
        {
            var request = await _unitOfWork.ResetPasswords.GetResetRequestByIdAsync(id);

            return request;
        }
        public async Task<ResetPasswordModel> CreateResetRequest(ResetPasswordRequestModel model)
        {

            Staffuser user = await _unitOfWork.Staffusers.GetStaffUserByEmailAsync(model.Email);

            if (user == null)
            {
                throw new InvalidOperationException("No user found with this email.");
            }

            if (user.Locked == (int)UserAccountStatus.Locked)
            {
                throw new InvalidOperationException(" Your account has been locked. Please contact system administrator");
            }

            Resetpassword resetRequest = new Resetpassword();
            resetRequest.Email = model.Email;
            resetRequest.UserId = user.UserId;
            resetRequest.RequestDate = DateTime.Now;
            resetRequest.Status = (int)Status.Active;
            resetRequest.Token = GeneratePasswordResetToken();
            resetRequest.IpAddress = model.IpAddress;
            try
            {
                await _unitOfWork.ResetPasswords.AddAsync(resetRequest);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            var response = new ResetPasswordModel();

            response.Id = resetRequest.Id;
            response.OrganizationName = user.Organization.Name;
            response.Name = $"{user.FirstName} {user.LastName}";
            response.Token = resetRequest.Token;
            response.Email = user.Email;
            var tenant = await _tenantService.GetTenantByOrganizationName(model.OrganizationName);
            response.ResetPasswordUrl = $"{tenant.BaseUrl}/#/resetpassword?organizationName={model.OrganizationName}&tokenId={Base64UrlEncoder.Encode(resetRequest.Token)}";
            try
            {
                await _emailService.SendPasswordResetNotification(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            return response;
        }
        public async Task<bool> ResetPassword(ResetPasswordRequestModel model)
        {
            if (model.Token == null || model.Token.Length == 0 )
            {
                throw new InvalidOperationException("Inavlid reset token. Please try again.");
            }
            var token = Base64UrlEncoder.Decode(model.Token);
            var resetRequest = await _unitOfWork.ResetPasswords.GetResetRequestByTokenAsync(token);

            if (resetRequest == null)
            {
                throw new InvalidOperationException("Inavlid reset token. Please try again.");
            }
            Staffuser user = await _unitOfWork.Staffusers.GetStaffUserByIdAsync(resetRequest.UserId);

            PasswordHash hash = new PasswordHash(model.Password);

            user.Salt = hash.Salt;
            user.Password = hash.Password;
            resetRequest.Status = (int)Status.InActive;
            try
            {
                _unitOfWork.ResetPasswords.Update(resetRequest);
                _unitOfWork.Staffusers.Update(user);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<bool> ValidateResetPasswordLink(ResetPasswordRequestModel model)
        {
            if (model.Token == null || model.Token.Length == 0  )
            {
                throw new InvalidOperationException("Inavlid reset token. Please try again.");
            }
            var token = Base64UrlEncoder.Decode(model.Token);
            var resetRequest = await _unitOfWork.ResetPasswords.GetResetRequestByTokenAsync(token);

            DateTime compareTime = DateTime.Now.AddMinutes(-30);
            DateTime requestTime = (DateTime)resetRequest.RequestDate;

            if (requestTime < compareTime)
            {
                throw new InvalidOperationException("The reset token has expired. Please send the reset password request again.");
            }

            if (resetRequest == null)
            {
                throw new InvalidOperationException("Inavlid reset token. Please try again.");
            }
            return true;
        }

        private string GeneratePasswordResetToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public async Task<ResetPasswordModel> CreateMemberPasswordResetRequest(ResetPasswordRequestModel model)
        {
            Person person = new Person();
            Company company = new Company();
            Entity entity = await _unitOfWork.Entities.GetEntityByUserNameAsync(model.Email);

            if (entity == null)
            {
                person = await _unitOfWork.Persons.GetPersonByEmailIdAsync(model.Email);
                company = await _unitOfWork.Companies.GetCompanyByEmailIdAsync(model.Email);
                if (person == null && company == null)
                {
                   throw new InvalidOperationException("No user found with this email or username.");
                }

                if (person != null)
                {
                    entity = await _unitOfWork.Entities.GetByIdAsync(person.EntityId ?? 0);
                }
                else
                {
                    entity = await _unitOfWork.Entities.GetByIdAsync(company.EntityId ?? 0);
                }

                if(entity.AccountLocked == (int)Status.Active)
                {
                    throw new InvalidOperationException("Your account is locked. Please contact the support team.");
                }
            }
            else
            {
                person = await _unitOfWork.Persons.GetPersonByEntityIdAsync(entity.EntityId);
            }

            Resetpassword resetRequest = new Resetpassword();
            resetRequest.EntityId = entity != null ? entity.EntityId : person.EntityId ?? 0;
            resetRequest.RequestDate = DateTime.Now;
            resetRequest.Status = (int)Status.Active;
            resetRequest.Token = GeneratePasswordResetToken();
            resetRequest.IpAddress = model.IpAddress;
            var email = "";
            var name = "";
            if (person != null)
            {
                PersonModel personModel = _mapper.Map<PersonModel>(person);
                resetRequest.Email = personModel.Emails.GetPrimaryEmail();
                email = resetRequest.Email;
                name = $"{personModel.FirstName + " " + personModel.LastName}";
            }
            else if(company != null)
            {
                var companyModel = _mapper.Map<CompanyModel>(company);
                var primaryEmail = await _unitOfWork.Emails.GetPrimaryEmailByCompanyId(companyModel.CompanyId);
                if (primaryEmail != null)
                {
                    resetRequest.Email = primaryEmail.EmailAddress;
                }
                email = resetRequest.Email;
                name = $"{companyModel.CompanyName}";
            }
            try
            {
                await _unitOfWork.ResetPasswords.AddAsync(resetRequest);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            var response = new ResetPasswordModel();

            response.Id = resetRequest.Id;
            response.Name = name;
            response.Token = resetRequest.Token;
            response.Email = email;
           
            return response;
        }

        public async Task<bool> ResetMemberPassword(ResetPasswordRequestModel model)
        {
            if (model.Token == null || model.Token.Length == 0  )
            {
                throw new InvalidOperationException("Invalid reset token. Please try again.");
            }
            var token = Base64UrlEncoder.Decode(model.Token);
            var resetRequest = await _unitOfWork.ResetPasswords.GetResetRequestByTokenAsync(model.Token);

            if (resetRequest == null)
            {
                throw new InvalidOperationException("Invalid reset request. Please try again.");
            }

            DateTime compareTime = DateTime.Now.AddMinutes(-30);
            DateTime requestTime = (DateTime)resetRequest.RequestDate;

            if (requestTime < compareTime)
            {
                throw new InvalidOperationException("The reset link has expired. Please send the reset password request again.");
            }

            Entity entity = await _unitOfWork.Entities.GetEntityByIdAsync(resetRequest.EntityId);

            if(entity.AccountLocked == (int)Status.Active)
            {
                throw new InvalidOperationException("Your account is locked. Please contact the support team.");
            }

            PasswordHash hash = new PasswordHash(model.Password);

            entity.WebPasswordSalt = hash.Salt;
            entity.WebPassword = hash.Password;
            entity.AccountLocked = (int)Status.InActive;
            entity.PasswordFailedAttempts = 0;
            resetRequest.Status = (int)Status.InActive;
            try
            {
                _unitOfWork.ResetPasswords.Update(resetRequest);
                _unitOfWork.Entities.Update(entity);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
