
using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Max.Services
{
    public class StaffUserService : IStaffUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISociableService _sociableService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        static readonly ILogger _logger = Serilog.Log.ForContext<StaffUserService>();
        public StaffUserService(IUnitOfWork unitOfWork, IMapper mapper, ISociableService sociableService, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._sociableService = sociableService;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<Staffuser> CreateStaffUser(StaffUserModel staffUserModel)
        {
            Staffuser staff = new Staffuser();
            bool isContentManager = false;
            if (staffUserModel.OrganizationId == 0)
            {
                var organization = await _unitOfWork.Organizations.GetAllOrganizationsAsync();

                if (organization != null)
                {
                    staffUserModel.OrganizationId = organization.FirstOrDefault().OrganizationId;
                }
            }

            var isValidUser = await ValidStaffUser(staffUserModel);
            if (isValidUser)
            {
                //Get HashedePassword & Salt value

                PasswordHash hash = new PasswordHash(staffUserModel.Password);
                //Map Model Data
                staff.UserName = staffUserModel.UserName;
                staff.FirstName = staffUserModel.FirstName;
                staff.LastName = staffUserModel.LastName;
                staff.Password = hash.Password;
                staff.Salt = hash.Salt;
                staff.Email = staffUserModel.Email;
                staff.DepartmentId = staffUserModel.DepartmentId;
                staff.LastAccessed = Constants.MySQL_MinDate;
                staff.Status = staffUserModel.Status;
                staff.OrganizationId = staffUserModel.OrganizationId;
                staff.Locked = staffUserModel.Locked;
                staff.CellPhoneNumber = staffUserModel.CellPhoneNumber.GetCleanPhoneNumber();
                staff.IsDisplayUser = (int)Status.Active;
                // Add roles
                var contentManagerRole = await _unitOfWork.Roles.GetRoleByNameAsync("Content Manager");
                foreach (var role in staffUserModel.SelectedRoles)
                {
                    staff.Staffroles.Add(new Staffrole { RoleId = role });
                    if (contentManagerRole != null)
                    {
                        if (staffUserModel.SelectedRoles.Contains(contentManagerRole.RoleId))
                        {
                            isContentManager = true;
                        }
                    }
                }

                try
                {
                    await _unitOfWork.Staffusers.AddAsync(staff);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error($"CreateStaffUser Error:{ex.Message} {ex.StackTrace}");
                    throw new Exception("Failed to create Staff User");
                }

                staffUserModel.UserId = staff.UserId;

                //Check if SocialSync is enabled
                var staffContext = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(staffUserModel.OrganizationId);
                if (configuration.SociableSyncEnabled == (int)Status.Active && staffContext != null)
                {
                    try
                    {
                        var sociableApiResponseModel = await _sociableService.CreateStaffUser(staffUserModel, staffUserModel.OrganizationId, isContentManager);

                        if (sociableApiResponseModel.SociableUserId > 0)
                        {
                            staff.SociableUserId = sociableApiResponseModel.SociableUserId;

                            //Get ProfileId and update Profile

                            var userInfo = await _sociableService.GetUserById(staff.SociableUserId ?? 0, staffUserModel.OrganizationId);

                            dynamic profile = JObject.Parse(userInfo);
                            var profileId = profile.profile_profiles[0].target_id;
                            if (profileId > 0)
                            {
                                sociableApiResponseModel = await _sociableService.UpdateUserProfile(staffUserModel, (int)profileId, staffUserModel.OrganizationId);
                                if (sociableApiResponseModel.ResponseStatus.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    throw new Exception($"Failed to update Profile: {sociableApiResponseModel.ResponseStatus.Message}");
                                }
                            }
                            staff.SociableProfileId = profileId;

                            _unitOfWork.Staffusers.Update(staff);
                            await _unitOfWork.CommitAsync();
                        }
                    }

                    catch (Exception ex)
                    {
                        _logger.Error($"CreateStaffUser Error:{ex.Message} {ex.StackTrace}");
                        throw new Exception("Failed to create Sociable Profile");
                    }
                }

            }
            return staff;
        }

        public async Task<List<StaffUserModel>> GetAllStaffUsers()
        {
            var users = await _unitOfWork.Staffusers.GetAllStaffUsersAsync();
            var model = _mapper.Map<List<StaffUserModel>>(users);

            //model = users.Where(x => x.IsDisplayUser == (int)Status.Active)
            model = users
                .Select(x => new StaffUserModel()
                {
                    UserId = x.UserId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    OrganizationId = x.OrganizationId,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    Email = x.Email,
                    UserName = x.UserName,
                    Status = x.Status,
                    Locked = x.Locked,
                    CellPhoneNumber = x.CellPhoneNumber,
                    Roles = x.Staffroles.Select(x => new RoleModel
                    {
                        RoleId = x.RoleId,
                        Name = x.Role.Name,
                        Description = x.Role.Description,
                        Status = x.Role.Status ?? 0
                    }).ToList(),
                    RoleNames = x.Staffroles.Select(f => f.Role.Name).ToArray()
                }).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

            return model;
        }

        public async Task<Staffuser> GetStaffUserById(int id)
        {
            return await _unitOfWork.Staffusers
                .GetStaffUserByIdAsync(id);
        }
        public async Task<Staffuser> GetStaffUserByName(string userName)
        {
            return await _unitOfWork.Staffusers
                .GetStaffUserByUserNameAsync(userName);
        }

        public async Task<bool> UpdateStaffUser(StaffUserModel staffUserModel)
        {
            var isValidUser = await ValidStaffUser(staffUserModel);
            var isContentManager = false;
            var isNewContentManager = false;
            var removeContentManager = true;
            Boolean needsSociableAccountUpdate = false;
            Boolean needsSociableProfileUpdate = false;
            if (isValidUser)
            {
                var staffUser = await _unitOfWork.Staffusers.GetStaffUserByIdAsync(staffUserModel.UserId);

                if (staffUser != null)
                {
                    if (staffUserModel.UserName != staffUser.UserName || staffUserModel.Password.Length > 0 || staffUserModel.Email != staffUser.Email)
                    {
                        needsSociableAccountUpdate = true;
                    }

                    if (staffUserModel.FirstName != staffUser.FirstName || staffUserModel.LastName != staffUser.LastName || staffUserModel.CellPhoneNumber != staffUser.CellPhoneNumber)
                    {
                        needsSociableProfileUpdate = true;
                    }
                    staffUser.UserName = staffUserModel.UserName;
                    staffUser.FirstName = staffUserModel.FirstName;
                    staffUser.LastName = staffUserModel.LastName;
                    staffUser.Email = staffUserModel.Email;
                    staffUser.Status = staffUserModel.Status;
                    staffUser.DepartmentId = staffUserModel.DepartmentId;

                    if (staffUserModel.Locked == 0 && staffUser.Locked == 1)
                    {
                        staffUser.FailedAttempts = 0;
                    }
                    staffUser.Locked = staffUserModel.Locked;
                    staffUser.CellPhoneNumber = staffUserModel.CellPhoneNumber.GetCleanPhoneNumber();

                    if (staffUserModel.Password.Length > 0)
                    {
                        await ResetPassword(staffUserModel.UserId, staffUserModel.Password);
                    }

                    var contentManagerRole = await _unitOfWork.Roles.GetRoleByNameAsync("Content Manager");


                    // Remove non selected roles
                    var staffRroles = staffUser.Staffroles;

                    if (contentManagerRole != null)
                    {
                        if (staffRroles.Any(x => x.RoleId == contentManagerRole.RoleId))
                        {
                            isContentManager = true;
                        }
                        if (staffUserModel.SelectedRoles.Contains(contentManagerRole.RoleId))
                        {
                            removeContentManager = false;
                        }
                    }
                    //Check if conent manager role has been removed
                    if (isContentManager && removeContentManager)
                    {
                        needsSociableAccountUpdate = true;
                    }

                    foreach (var staffRole in staffRroles)
                    {
                        _unitOfWork.Staffroles.Remove(staffRole);
                    }

                    // Add roles
                    foreach (var role in staffUserModel.SelectedRoles)
                    {
                        staffUser.Staffroles.Add(new Staffrole { RoleId = role });

                        //check if content manager role is added
                        if (contentManagerRole != null)
                        {
                            if (contentManagerRole.RoleId == role && !isContentManager)
                            {
                                isNewContentManager = true;
                                needsSociableAccountUpdate = true;
                            }
                        }

                    }
                }

                //Check if SocialSync is enabled
                var staffContext = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(staffUserModel.OrganizationId);
                if (configuration.SociableSyncEnabled == (int)Status.Active && staffContext != null)
                {
                    if (staffUser.SociableUserId > 0)
                    {
                        //update StaffUser
                        if (needsSociableAccountUpdate)
                        {
                            try
                            {
                               var userInfoExist = await _sociableService.GetUserById(Convert.ToInt32(staffUser.SociableUserId), staffContext.OrganizationId);
                               dynamic uInfo = JObject.Parse(userInfoExist);
                               if(uInfo.uid == null)
                               {
                                    var sociableApiResponseModel = await _sociableService.CreateStaffUser(staffUserModel, staffUserModel.OrganizationId, isContentManager);
                                    staffUser.SociableUserId = sociableApiResponseModel.SociableUserId;
                                }
                                var userInfo = await _sociableService.GetUserById(staffUser.SociableUserId ?? 0, staffUserModel.OrganizationId);
                                dynamic profile = JObject.Parse(userInfo);
                                if (profile.uid != null)
                                {
                                    var updateStaffUser =await _sociableService.UpdateStaffUser(staffUser.SociableUserId ?? 0, staffUserModel.UserName, staffUserModel.Password, staffUserModel.Email, staffUser.OrganizationId, isContentManager, isNewContentManager);
                                    staffUser.SociableProfileId = updateStaffUser.SociableProfileId;
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Error($"UpdateStaffUser Error:{ex.Message} {ex.StackTrace}");
                                throw new Exception("Failed to update Sociable User");
                            }
                        }
                        if (staffUser.SociableProfileId > 0)
                        {
                            //Update Staff User Profile
                            if (needsSociableProfileUpdate)
                            {
                                try
                                {
                                    await _sociableService.UpdateUserProfile(staffUserModel, staffUser.SociableProfileId ?? 0, staffUserModel.OrganizationId);
                                }

                                catch (Exception ex)
                                {
                                    _logger.Error($"UpdateStaffUser Error:{ex.Message} {ex.StackTrace}");
                                    throw new Exception("Failed to update Sociable Profile");
                                }
                            }
                        }
                    }
                    else
                    {
                        //create sociableUser

                        try
                        {
                            var sociableApiResponseModel = await _sociableService.CreateStaffUser(staffUserModel, staffUserModel.OrganizationId, isContentManager);

                            if (sociableApiResponseModel.SociableUserId > 0)
                            {
                                staffUser.SociableUserId = sociableApiResponseModel.SociableUserId;

                                //Get ProfileId and update Profile

                                var userInfo = await _sociableService.GetUserById(sociableApiResponseModel.SociableUserId, staffUserModel.OrganizationId);

                                dynamic profile = JObject.Parse(userInfo);
                                var profileId = profile.profile_profiles[0].target_id;
                                if (profileId > 0)
                                {

                                    sociableApiResponseModel = await _sociableService.UpdateUserProfile(staffUserModel, (int)profileId, staffUserModel.OrganizationId);
                                    if (sociableApiResponseModel.ResponseStatus.StatusCode != System.Net.HttpStatusCode.OK)
                                    {
                                        throw new Exception($"Failed to update Profile: {sociableApiResponseModel.ResponseStatus.Message}");
                                    }
                                }
                                staffUser.SociableProfileId = profileId;
                            }
                            else
                            {
                                _logger.Error($"UpdateStaffUser: Failed to create Sociable profile");
                                throw new Exception("Failed to create Sociable Profile");
                            }
                        }

                        catch (Exception ex)
                        {
                            _logger.Error($"UpdateStaffUser Error:{ex.Message} {ex.StackTrace}");
                            throw new Exception("Failed to create Sociable Profile");
                        }
                    }

                }
                try
                {
                    _unitOfWork.Staffusers.Update(staffUser);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error($"UpdateStaffUser Error:{ex.Message} {ex.StackTrace}");
                    throw new Exception("Failed to update user information.");
                }

                return true;
            }
            return false;
        }

        public async Task<bool> ResetPassword(int staffUserId, string password)
        {
            bool isConentManager = false;
            var staffUser = await _unitOfWork.Staffusers.GetByIdAsync(staffUserId);
            var contentManagerRole = await _unitOfWork.Roles.GetRoleByNameAsync("Content Manager");
            if (contentManagerRole != null)
            {
                var roles = await _unitOfWork.Staffroles.GetAllStaffRolesByStaffIdAsync(staffUser.UserId);
                if (roles.Any(x => x.RoleId == contentManagerRole.RoleId))
                {
                    isConentManager = true;
                }
            }

            if (staffUser != null)
            {
                PasswordHash hash = new PasswordHash(password);
                var isValidPassword = hash.IsValidPassword(staffUser.Salt, staffUser.Password, password);
                if (isValidPassword)
                {
                    throw new InvalidOperationException("New password cannot be same as existing password.");
                }
                var staffContext = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(staffUser.OrganizationId);
                if (configuration.SociableSyncEnabled == (int)Status.Active && staffContext != null)
                {
                    if (staffUser.SociableProfileId > 0)
                    {
                        try
                        {
                            var userInfoExist = await _sociableService.GetUserById(Convert.ToInt32(staffUser.SociableUserId), staffContext.OrganizationId);
                            dynamic uInfo = JObject.Parse(userInfoExist);
                            var staffUserModel = _mapper.Map<StaffUserModel>(staffUser);
                            if (uInfo.uid == null)
                            {
                                var sociableApiResponseModel = await _sociableService.CreateStaffUser(staffUserModel, staffUserModel.OrganizationId, isConentManager);
                                staffUser.SociableUserId = sociableApiResponseModel.SociableUserId;
                            }
                            var userInfo = await _sociableService.GetUserById(staffUser.SociableUserId ?? 0, staffUserModel.OrganizationId);
                            dynamic profile = JObject.Parse(userInfo);
                            if (profile.uid != null)
                            {
                                var updateStaffUser = await _sociableService.UpdateStaffUser(staffUser.SociableUserId ?? 0, staffUser.UserName, password, staffUser.Email, staffUser.OrganizationId, isConentManager, false);
                                staffUser.SociableProfileId = updateStaffUser.SociableProfileId;
                            }
                            _unitOfWork.Staffusers.Update(staffUser);
                            await _unitOfWork.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"ResetPassword Error:{ex.Message} {ex.StackTrace}");
                            throw new Exception("Failed to update user information.");
                        }
                    }
                }
                staffUser.Salt = hash.Salt;
                staffUser.Password = hash.Password;

                try
                {
                    _unitOfWork.Staffusers.Update(staffUser);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error($"ResetPassword Error:{ex.Message} {ex.StackTrace}");
                    throw new Exception("Failed to update user information.");
                }

            }
            return false;
        }
        public async Task<bool> DeleteStaffUser(int staffUserId)
        {

            //Check if user has any document history
            var accessHistory = _unitOfWork.DocumentObjectAccessHistories.Find(x => x.StaffUserId == staffUserId);
            if (accessHistory.Count() > 0)
            {
                _logger.Information($"DeleteStaffUser: there are document history attached to Staff User {staffUserId}");
                throw new InvalidOperationException("User can not be deleted as there are linked document history. You can make him InActive.");
            }

            //Check if user has any transaction.Is so then do not delete

            var receipts = _unitOfWork.ReceiptHeaders.GetReceiptsByStaffIdAsync(staffUserId);
            if (receipts != null)
            {
                if (receipts.Result.Any())
                {
                    _logger.Information($"DeleteStaffUser: there are receipts attached to Staff User {staffUserId}");
                    throw new InvalidOperationException("User can not be deleted as there are linked transactions. You can make him InActive.");
                }
            }

            var reports = _unitOfWork.Reports.GetMembershipReportsByUserAsync(staffUserId);
            if (reports != null)
            {
                if (reports.Result.Any())
                {
                    _logger.Information($"DeleteStaffUser: there are reports attached to Staff User {staffUserId}");
                    throw new InvalidOperationException("User can not be deleted as there are linked Reports. You can make him InActive.");
                }
            }
            // Delete all chidl records from staffRoles
            var staffRoles = await _unitOfWork.Staffroles.GetAllStaffRolesByStaffIdAsync(staffUserId);
            if (staffRoles != null)
            {
                _unitOfWork.Staffroles.RemoveRange(staffRoles);
            }
            // Delete Staff User
            var staffUser = await _unitOfWork.Staffusers.GetByIdAsync(staffUserId);
            if (staffUser != null)
            {
                try
                {
                    _unitOfWork.Staffusers.Remove(staffUser);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error($"DeleteStaffUser Error:{ex.Message} {ex.StackTrace}");
                    throw new Exception("Failed to delete the user.");
                }
            }
            return false;
        }
        public async Task<bool> AssignRole(int staffUserId, int roleId)
        {
            var staffUser = await _unitOfWork.Staffusers.GetByIdAsync(staffUserId);
            // _unitOfWork.Staffusers.Remove(staffuser);
            await _unitOfWork.CommitAsync();
            return true;
        }
        public async Task<int> UpdateLoginStatus(int userId, int status)
        {
            var staffUser = await _unitOfWork.Staffusers.GetByIdAsync(userId);

            if (status == (int)LoginStatus.Success)
            {
                staffUser.FailedAttempts = 0;
                staffUser.LastAccessed = DateTime.Now;
            }
            else
            {
                staffUser.FailedAttempts += 1;
                if ((Constants.MAX_FAILED_ATTEMPTS - staffUser.FailedAttempts) <= 0)
                {
                    staffUser.Locked = (int)UserAccountStatus.Locked;
                }
            }
            try
            {
                _unitOfWork.Staffusers.Update(staffUser);
                await _unitOfWork.CommitAsync();
                return staffUser.FailedAttempts;
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateLoginStatus Error:{ex.Message} {ex.StackTrace}");
                throw new Exception("Failed to update the user login status.");
            }
        }
        public async Task<int> UpdateLoginStatus(Multifactorcode code, int rememberDevice, int status)
        {
            var staffUser = await _unitOfWork.Staffusers.GetByIdAsync(code.UserId);
            var device = await _unitOfWork.UserDevices.GetByIdAsync(code.DeviceId ?? 0);

            if (status == (int)LoginStatus.Success)
            {
                staffUser.FailedAttempts = 0;
                staffUser.LastAccessed = DateTime.Now;
                device.LastAccessed = DateTime.Now;
                device.Authenticated = (int)LoginStatus.Success;
                device.RemberDevice = rememberDevice;
                device.LastValidated = DateTime.Now;
                _unitOfWork.UserDevices.Update(device);
            }
            else
            {
                staffUser.FailedAttempts += 1;
                if ((Constants.MAX_FAILED_ATTEMPTS - staffUser.FailedAttempts) <= 0)
                {
                    staffUser.Locked = (int)UserAccountStatus.Locked;
                }
            }
            try
            {
                _unitOfWork.MultiFactorCodes.Update(code);
                _unitOfWork.Staffusers.Update(staffUser);
                await _unitOfWork.CommitAsync();
                return staffUser.FailedAttempts;
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateLoginStatus Error:{ex.Message} {ex.StackTrace}");
                throw new Exception("Failed to update the user login status.");
            }
        }
        public async Task<List<SelectListModel>> GetStaffUserSelectList()
        {
            var users = await _unitOfWork.Staffusers.GetAllStaffUsersAsync();
            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var user in users.OrderBy(x => x.FirstName).ThenBy(x => x.LastName))
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = user.UserId.ToString();
                selectListItem.name = $"{user.FirstName} {user.LastName}";
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public async Task<List<StaffUserModel>> GetAllStaffUsersByNameAndUserNameAndEmail(string value)
        {
            var users = await _unitOfWork.Staffusers.GetStaffByFirstORLastNameAsync(value);
            var model = _mapper.Map<List<StaffUserModel>>(users);

            return model;
        }

        private async Task<bool> ValidStaffUser(StaffUserModel staffUser)
        {
            //Validate User Name
            if (staffUser.UserName == null)
            {
                throw new NullReferenceException($"User Name can not be NULL.");
            }

            if (staffUser.UserName.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"User Name can not be empty.");
            }

            //Check if User already exists

            var user = await _unitOfWork.Staffusers.GetStaffUserByUserNameAsync(staffUser.UserName);

            if (user != null)
            {
                //check id ID & UserName are same -> Updating
                if (user.UserId != staffUser.UserId)
                {
                    throw new InvalidOperationException($"Duplicate User Name.");
                }

            }

            //Validate Email
            if (staffUser.Email == null)
            {
                throw new NullReferenceException($"Email can not be NULL.");
            }

            if (staffUser.Email.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"Email can not be empty.");
            }

            //Check if email already exists

            user = await _unitOfWork.Staffusers.GetStaffUserByEmailAsync(staffUser.Email);

            if (user != null)
            {
                if (user.UserId != staffUser.UserId)
                {
                    throw new InvalidOperationException($"Duplicate Email.");
                }
            }


            //Check if email already exists for person

            var person = await _unitOfWork.Persons.GetPersonByEmailIdAsync(staffUser.Email);

            if (person != null)
            {
                throw new InvalidOperationException($"A person already exists with this Email.");
            }


            //Check if email already exists for company

            var company = await _unitOfWork.Companies.GetCompanyByEmailIdAsync(staffUser.Email);

            if (company != null)
            {
                throw new InvalidOperationException($"A company already exists with this Email.");
            }

            return true;
        }
    }
}