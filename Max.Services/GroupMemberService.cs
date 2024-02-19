using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Repositories;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
using System.Linq;
using System.Security.Cryptography;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Newtonsoft.Json.Linq;

namespace Max.Services
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupMemberService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEntityService _entityService;
        private readonly IGroupRoleService _groupRoleService;
        private readonly ICommanService _commanService;
        private readonly ISociableService _sociableService;
        private readonly ISociableGroupService _sociableGroupService;
        public GroupMemberService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GroupMemberService> logger, IHttpContextAccessor httpContextAccessor, IEntityService entityService, IGroupRoleService groupRoleService, ICommanService commanService, ISociableService sociableService, ISociableGroupService sociableGroupService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _entityService = entityService;
            _groupRoleService = groupRoleService;
            _commanService = commanService;
            _sociableService = sociableService;
            _sociableGroupService = sociableGroupService;
        }

        public async Task<Groupmember> AddGroupMember(GroupMemberModel groupmembermodel)
        {
            var isValid = await ValidateGroupMember(groupmembermodel);

            Groupmember groupmember = new Groupmember();
            if (isValid)
            {
                try
                {
                    List<string> groupRoleList = new List<string>();
                    var userId = _commanService.GetUserIdFromContext();
                    var group = await _unitOfWork.Groups.GetByIdAsync(groupmembermodel.GroupId ?? 0);
                    var entity = await _entityService.GetEntityById(groupmembermodel.EntityId ?? 0);

                    groupmember.GroupId = groupmembermodel.GroupId;
                    groupmember.EntityId = groupmembermodel.EntityId;
                    groupmember.Status = groupmembermodel.Status;

                    await _unitOfWork.GroupMembers.AddAsync(groupmember);
                    await _unitOfWork.CommitAsync();

                    Grouphistory grouphistory = new Grouphistory();
                    grouphistory.GroupId = group.GroupId;
                    grouphistory.GroupMemberId = groupmember.GroupMemberId;
                    grouphistory.ActivityDate = DateTime.Now;
                    grouphistory.ActivityType = "Created";
                    grouphistory.ActivityStaffId = userId; // ActivityStaffId could be staffId or entityId
                    grouphistory.ActivityDescription = $"Added person '{entity.Name}' to group '{group.GroupName}'.";

                    await _unitOfWork.GroupHistories.AddAsync(grouphistory);

                    List<Grouphistory> listGrouphistory = new List<Grouphistory>();
                    List<Groupmemberrole> listGroupMemberRole = new List<Groupmemberrole>();
                    foreach (var role in groupmembermodel.GroupMemberRoles)
                    {
                        Groupmemberrole groupMemberRole = new Groupmemberrole();
                        groupMemberRole.GroupMemberId = groupmember.GroupMemberId;
                        groupMemberRole.GroupRoleId = role.GroupRoleId;
                        groupMemberRole.StartDate = role.StartDate?.Date;
                        groupMemberRole.EndDate = role.EndDate?.Date;
                        groupMemberRole.Status = role.Status;
                        listGroupMemberRole.Add(groupMemberRole);

                        var groupRole = await _groupRoleService.GetGroupRolesById(role.GroupRoleId ?? 0);
                        groupRoleList.Add(groupRole.GroupRoleName);

                        Grouphistory groupHistoryRoles = new Grouphistory();
                        groupHistoryRoles.GroupId = groupmembermodel.GroupId;
                        groupHistoryRoles.GroupMemberId = groupmember.GroupMemberId;
                        groupHistoryRoles.GroupRoleId = role.GroupRoleId;
                        groupHistoryRoles.ActivityDate = DateTime.Now;
                        groupHistoryRoles.ActivityType = "Created";
                        groupHistoryRoles.ActivityStaffId = userId; // ActivityStaffId could be staffId or entityId
                        groupHistoryRoles.ActivityDescription = $"Added role '{groupRole.GroupRoleName}' for '{entity.Name}' for group '{group.GroupName}'.";
                        listGrouphistory.Add(groupHistoryRoles);
                    }

                    await _unitOfWork.GroupMemberRoles.AddRangeAsync(listGroupMemberRole);
                    await _unitOfWork.GroupHistories.AddRangeAsync(listGrouphistory);
                    await _unitOfWork.CommitAsync();

                    var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                    var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(staff.OrganizationId);
                    if (configuration.SociableSyncEnabled == (int)Status.Active && staff != null)
                    {
                        await CreateEntityAndGroupToSocial(groupmembermodel.EntityId, entity, group);

                        if (group.SocialGroupId != null)
                        {
                            await CreateUpdateGroupMemberToSocial(groupmembermodel.EntityId, staff.OrganizationId, group.SocialGroupId, groupRoleList, groupmember, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }

            }

            return groupmember;
        }

        public async Task<bool> AddSocialGroupMember(AddSocialGroupMember groupmembermodel)
        {
            var isSuccess = false;
            int? entityId = null;
            Groupmember groupmember = new Groupmember();
            try
            {
                if (groupmembermodel.entity_id.Any())
                {
                    entityId = Convert.ToInt32(groupmembermodel.entity_id[0].target_id);

                    if (groupmembermodel.group_id.Any())
                    {
                        var groupDetails = await _unitOfWork.Groups.GetByIdAsync(Convert.ToInt32(groupmembermodel.group_id[0].value));
                        if (groupDetails != null)
                        {
                            groupmember.GroupId = groupDetails.GroupId;
                            groupmember.EntityId = entityId;
                            groupmember.Status = (int)Status.Active;
                            await _unitOfWork.GroupMembers.AddAsync(groupmember);
                            await _unitOfWork.CommitAsync();

                            var userId = _commanService.GetUserIdFromContext();
                            var entity = await _entityService.GetEntityById(Convert.ToInt32(entityId));

                            Grouphistory grouphistory = new Grouphistory();
                            grouphistory.GroupId = groupDetails.GroupId;
                            grouphistory.GroupMemberId = groupmember.GroupMemberId;
                            grouphistory.ActivityDate = DateTime.Now;
                            grouphistory.ActivityType = "Created";
                            grouphistory.ActivityStaffId = userId; // ActivityStaffId could be staffId or entityId
                            grouphistory.ActivityDescription = $"Added person '{entity.Name}' to group '{groupDetails.GroupName}'.";
                            await _unitOfWork.GroupHistories.AddAsync(grouphistory);

                            if (groupmembermodel.group_roles.Any())
                            {
                                List<Grouphistory> listGrouphistory = new List<Grouphistory>();
                                List<Groupmemberrole> listGroupMemberRole = new List<Groupmemberrole>();
                                foreach (var role in groupmembermodel.group_roles)
                                {
                                    var roleDetails = await _unitOfWork.GroupRoles.GetGroupRoleByNameAsync(role.target_id);
                                    if (roleDetails != null)
                                    {
                                        Groupmemberrole groupMemberRole = new Groupmemberrole();
                                        groupMemberRole.GroupMemberId = groupmember.GroupMemberId;
                                        groupMemberRole.GroupRoleId = roleDetails.GroupRoleId;
                                        groupMemberRole.StartDate = groupDetails.TerrmStartDate?.Date;
                                        groupMemberRole.EndDate = groupDetails.TermEndDate?.Date;
                                        groupMemberRole.Status = (int)Status.Active;
                                        listGroupMemberRole.Add(groupMemberRole);

                                        Grouphistory groupHistoryRoles = new Grouphistory();
                                        groupHistoryRoles.GroupId = groupDetails.GroupId;
                                        groupHistoryRoles.GroupMemberId = groupmember.GroupMemberId;
                                        groupHistoryRoles.GroupRoleId = roleDetails.GroupRoleId;
                                        groupHistoryRoles.ActivityDate = DateTime.Now;
                                        groupHistoryRoles.ActivityType = "Created";
                                        groupHistoryRoles.ActivityStaffId = userId; // ActivityStaffId could be staffId or entityId
                                        groupHistoryRoles.ActivityDescription = $"Added role '{roleDetails.GroupRoleName}' for '{entity.Name}' for group '{groupDetails.GroupName}'.";
                                        listGrouphistory.Add(groupHistoryRoles);
                                    }
                                }

                                await _unitOfWork.GroupMemberRoles.AddRangeAsync(listGroupMemberRole);
                                await _unitOfWork.GroupHistories.AddRangeAsync(listGrouphistory);
                                await _unitOfWork.CommitAsync();
                                isSuccess = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return isSuccess;
        }

        public async Task<List<GroupMemberViewModel>> GetAllGroupMembersByGroupId(int groupId)
        {
            var groupMembers = await _unitOfWork.GroupMembers.GetAllGroupMembersByGroupIdAsync(groupId);
            List<GroupMemberViewModel> groupMemberModelList = new List<GroupMemberViewModel>();

            foreach (var groupmember in groupMembers)
            {
                try
                {
                    GroupMemberViewModel groupMemberModel = new GroupMemberViewModel();
                    groupMemberModel.GroupMemberId = groupmember.GroupMemberId;
                    groupMemberModel.Status = groupmember.Status;
                    groupMemberModel.EntityId = groupmember.EntityId;
                    groupMemberModel.GroupId = groupId;
                    if (groupmember.Entity.PersonId != null && groupmember.Entity.People.ToList().Count > 0)
                    {
                        groupMemberModel.Email = groupmember.Entity.People.ToList()[0].Emails.Where(x => x.IsPrimary == 1).Select(x => x.EmailAddress).FirstOrDefault();
                        groupMemberModel.CellPhoneNumber = groupmember.Entity.People.ToList()[0].Phones.Where(x => x.IsPrimary == 1).Select(x => x.PhoneNumber).FirstOrDefault();
                        groupMemberModel.EntityName = groupmember.Entity.Name;
                        groupMemberModel.RoleName = groupmember.Groupmemberroles.Select(x => x.GroupRole.GroupRoleName).FirstOrDefault();
                    }
                    groupMemberModelList.Add(groupMemberModel);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }
            }
            return groupMemberModelList;
        }

        public async Task<List<GroupMemberModel>> GetAllGroupsByEntityId(int entityId)
        {
            var groupMembers = await _unitOfWork.GroupMembers.GetAllGroupsByEntityIdAsync(entityId);
            List<GroupMemberModel> groupMemberModelList = new List<GroupMemberModel>();

            foreach (var groupmember in groupMembers)
            {
                GroupMemberModel groupMemberModel = _mapper.Map<GroupMemberModel>(groupmember);
                //groupMemberModel.Email = groupmember.Person.Emails.Where(x => x.IsPrimary == 1).Select(x => x.EmailAddress).FirstOrDefault();
                //groupMemberModel.CellPhoneNumber = groupmember.Person.Phones.Where(x => x.IsPrimary == 1).Select(x => x.PhoneNumber).FirstOrDefault();
                //groupMemberModel.FirstName = groupmember.Person.FirstName;
                //groupMemberModel.LastName = groupmember.Person.LastName;
                //groupMemberModel.RoleName = groupmember.Role.GroupRoleName; 
                groupMemberModel.GroupName = groupmember.Group.GroupName;
                groupMemberModel.GroupDescription = groupmember.Group.GroupDescription;
                groupMemberModel.GroupStatus = groupmember.Group.Status ?? 0;
                groupMemberModelList.Add(groupMemberModel);
            }
            return groupMemberModelList;
        }

        public async Task<List<GroupSociableModel>> GetGroupsByEntityId(int entityId)
        {
            var groupMembers = await _unitOfWork.GroupMembers.GetGroupsByEntityIdAsync(entityId);
            List<GroupSociableModel> groupMemberModelList = new List<GroupSociableModel>();
            foreach (var groupmember in groupMembers)
            {
                GroupSociableModel groupSociableModel = new GroupSociableModel();
                groupSociableModel.GroupId = groupmember.Group.GroupId;
                groupSociableModel.GroupName = groupmember.Group.GroupName;
                groupSociableModel.Status = groupmember.Group.Status ?? 0;

                List<GroupRoleSociableModel> groupMemberSociableModelList = new List<GroupRoleSociableModel>();
                foreach (var role in groupmember.Groupmemberroles)
                {
                    GroupRoleSociableModel groupRoleSociableModel = new GroupRoleSociableModel();
                    groupRoleSociableModel.GroupRoleName = role.GroupRole.GroupRoleName;
                    groupRoleSociableModel.StartDate = role.StartDate;
                    groupRoleSociableModel.EndDate = role.EndDate;
                    groupRoleSociableModel.Status = role.Status;
                    groupSociableModel.Roles.Add(groupRoleSociableModel);
                }
                groupMemberModelList.Add(groupSociableModel);
            }
            return groupMemberModelList;
        }

        public async Task<bool> UpdateGroupMember(GroupMemberModel groupmembermodel)
        {
            var groupMemberRecord = await _unitOfWork.GroupMembers.GetGroupMemberByIdAsync(groupmembermodel.GroupMemberId);

            if (groupMemberRecord != null)
            {
                var userId = _commanService.GetUserIdFromContext();
                var group = await _unitOfWork.Groups.GetByIdAsync(groupmembermodel.GroupId ?? 0);
                var entity = await _entityService.GetEntityById(groupmembermodel.EntityId ?? 0);

                if (groupMemberRecord.Status != groupmembermodel.Status)
                {
                    groupMemberRecord.Status = groupmembermodel.Status;
                    _unitOfWork.GroupMembers.Update(groupMemberRecord);

                    Grouphistory grouphistory = new Grouphistory();
                    grouphistory.GroupId = groupmembermodel.GroupId;
                    grouphistory.GroupMemberId = groupmembermodel.GroupMemberId;
                    grouphistory.ActivityDate = DateTime.Now;
                    grouphistory.ActivityType = "Modified";
                    grouphistory.ActivityStaffId = userId; // ActivityStaffId could be staffId or entityId
                    if (groupmembermodel.Status == 1)
                    {
                        grouphistory.ActivityDescription = $"Changed status from Inactive to Active for member '{entity.Name}' for group '{group.GroupName}'.";
                    }
                    else
                    {
                        grouphistory.ActivityDescription = $"Changed status from Active to Inactive for member '{entity.Name}' for group '{group.GroupName}'.";
                    }
                    await _unitOfWork.GroupHistories.AddAsync(grouphistory);
                    await _unitOfWork.CommitAsync();
                }

                // Update Existing roles
                foreach (var role in groupmembermodel.GroupMemberRoles.Where(x => x.GroupMemberRoleId > 0))
                {
                    string activityDescription = string.Empty;

                    var groupMemberRole = await _unitOfWork.GroupMemberRoles.GetGroupMemberRoleById(role.GroupMemberRoleId);
                    if (groupMemberRole.GroupRoleId != role.GroupRoleId)
                    {
                        var groupRole = await _unitOfWork.GroupRoles.GetByIdAsync(role.GroupRoleId ?? 0);
                        groupMemberRole.GroupRoleId = role.GroupRoleId;
                        activityDescription = $"Changed role from '{groupMemberRole.GroupRole.GroupRoleName}' to '{groupRole.GroupRoleName}' for member '{entity.Name}' for group '{group.GroupName}'.";
                    }
                    if (groupMemberRole.StartDate != role.StartDate?.Date)
                    {
                        var oldStartDate = groupMemberRole.StartDate?.Date == null ? "" : groupMemberRole.StartDate.Value.ToShortDateString();
                        var newStartDate = role.StartDate?.Date == null ? "" : role.StartDate.Value.ToShortDateString();
                        groupMemberRole.StartDate = role.StartDate?.Date;
                        activityDescription = $"Changed start date from '{oldStartDate}' to '{newStartDate}' for member '{entity.Name}' for group '{group.GroupName}'.";
                    }
                    if (groupMemberRole.EndDate != role.EndDate?.Date)
                    {
                        var oldEndDate = groupMemberRole.EndDate?.Date == null ? "" : groupMemberRole.EndDate.Value.ToShortDateString();
                        var newEndDate = role.EndDate?.Date == null ? "" : role.EndDate.Value.ToShortDateString();
                        groupMemberRole.EndDate = role.EndDate?.Date;
                        activityDescription = $"Changed end date from '{oldEndDate}' to '{newEndDate}' for member '{entity.Name}' for group '{group.GroupName}'.";
                    }
                    if (groupMemberRecord.Status == 0)
                    {
                        groupMemberRole.Status = groupMemberRecord.Status;
                    }
                    else
                    {
                        if (groupMemberRole.Status != role.Status)
                        {
                            groupMemberRole.Status = role.Status;
                            if (role.Status == 1)
                            {
                                activityDescription = $"Changed status from Inactive to Active for member '{entity.Name}' for group '{group.GroupName}'.";
                            }
                            else
                            {
                                activityDescription = $"Changed status from Active to Inactive for member '{entity.Name}' for group '{group.GroupName}'.";
                            }
                        }
                    }

                    if (activityDescription.Length > 0)
                    {
                        Grouphistory grouphistory = new Grouphistory();
                        grouphistory.GroupId = groupmembermodel.GroupId;
                        grouphistory.GroupMemberId = groupmembermodel.GroupMemberId;
                        grouphistory.ActivityDate = DateTime.Now;
                        grouphistory.ActivityType = "Modified";
                        grouphistory.ActivityStaffId = userId; // ActivityStaffId could be staffId or entityId
                        grouphistory.ActivityDescription = activityDescription;
                        await _unitOfWork.GroupHistories.AddAsync(grouphistory);
                    }
                    _unitOfWork.GroupMemberRoles.Update(groupMemberRole);
                    await _unitOfWork.CommitAsync();
                }

                List<Grouphistory> listGrouphistory = new List<Grouphistory>();
                List<Groupmemberrole> listGroupMemberRole = new List<Groupmemberrole>();

                foreach (var role in groupmembermodel.GroupMemberRoles.Where(x => x.GroupMemberRoleId == 0))
                {
                    Groupmemberrole groupMemberRole = new Groupmemberrole();
                    groupMemberRole.GroupMemberId = role.GroupMemberId;
                    groupMemberRole.GroupRoleId = role.GroupRoleId;
                    groupMemberRole.StartDate = role.StartDate;
                    groupMemberRole.EndDate = role.EndDate;
                    groupMemberRole.Status = role.Status;
                    listGroupMemberRole.Add(groupMemberRole);

                    var groupRole = await _groupRoleService.GetGroupRolesById(role.GroupRoleId ?? 0);

                    Grouphistory groupHistoryRoles = new Grouphistory();
                    groupHistoryRoles.GroupId = groupmembermodel.GroupId;
                    groupHistoryRoles.GroupMemberId = role.GroupMemberId;
                    groupHistoryRoles.GroupRoleId = role.GroupRoleId;
                    groupHistoryRoles.ActivityDate = DateTime.Now;
                    groupHistoryRoles.ActivityType = "Created";
                    groupHistoryRoles.ActivityStaffId = userId; // ActivityStaffId could be staffId or entityId
                    groupHistoryRoles.ActivityDescription = $"Added role '{groupRole.GroupRoleName}' for '{entity.Name}' for group '{group.GroupName}'.";
                    listGrouphistory.Add(groupHistoryRoles);
                }

                try
                {
                    await _unitOfWork.GroupMemberRoles.AddRangeAsync(listGroupMemberRole);
                    await _unitOfWork.GroupHistories.AddRangeAsync(listGrouphistory);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }

                var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(staff.OrganizationId);
                if (configuration.SociableSyncEnabled == (int)Status.Active && staff != null)
                {
                    await CreateEntityAndGroupToSocial(groupmembermodel.EntityId, entity, group);

                    int? socialGroupId = group.SocialGroupId;
                    if (group.SocialGroupId != null)
                    {
                        List<string> groupRoleList = new List<string>();
                        var activeGroupRoleList = groupmembermodel.GroupMemberRoles.Where(x => x.Status == (int)Status.Active);
                        foreach (var item in activeGroupRoleList)
                        {
                            var groupRole = await _unitOfWork.GroupRoles.GetByIdAsync(item.GroupRoleId ?? 0);
                            if (groupRole != null)
                            {
                                groupRoleList.Add(groupRole.GroupRoleName);
                            }
                        }
                        if (groupMemberRecord.SocialGroupMemberId == null && groupRoleList.Any())
                        {
                            await CreateUpdateGroupMemberToSocial(groupmembermodel.EntityId, staff.OrganizationId, group.SocialGroupId, groupRoleList, groupMemberRecord, true);
                        }
                        else if (groupMemberRecord.SocialGroupMemberId != null)
                        {
                            await CreateUpdateGroupMemberToSocial(groupmembermodel.EntityId, staff.OrganizationId, group.SocialGroupId, groupRoleList, groupMemberRecord, false);
                        }
                    }
                }

                return true;
            }
            return false;
        }

        public async Task<bool> DeleteGroupMember(int groupMemberId)
        {
            // Delete Group Member Roles
            var groupMemberRole = await _unitOfWork.GroupMemberRoles.GetAllGroupMemberRolesByGroupMemberIdAsync(groupMemberId);
            if (groupMemberRole != null)
            {
                _unitOfWork.GroupMemberRoles.RemoveRange(groupMemberRole);
            }
            // Delete Group Member
            var groupMember = await _unitOfWork.GroupMembers.GetByIdAsync(groupMemberId);
            if (groupMember != null)
            {
                var userId = _commanService.GetUserIdFromContext();
                var group = await _unitOfWork.Groups.GetByIdAsync(groupMember.GroupId ?? 0);
                var entity = await _entityService.GetEntityById(groupMember.EntityId ?? 0);

                Grouphistory grouphistory = new Grouphistory();
                grouphistory.ActivityDate = DateTime.Now;
                grouphistory.ActivityType = "Deleted";
                grouphistory.ActivityStaffId = userId;
                grouphistory.ActivityDescription = $"Deleted member '{entity.Name}' from group '{group.GroupName}'.";
                await _unitOfWork.GroupHistories.AddAsync(grouphistory);

                _unitOfWork.GroupMembers.Remove(groupMember);
                await _unitOfWork.CommitAsync();

                var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(staff.OrganizationId);
                if (configuration.SociableSyncEnabled == (int)Status.Active && staff != null)
                {
                    if(groupMember.SocialGroupMemberId != null)
                    {
                        await _sociableGroupService.DeleteSocialGroupMembers(staff.OrganizationId, Convert.ToInt32(groupMember.SocialGroupMemberId), Convert.ToInt32(group.SocialGroupId));
                    }
                }

                return true;
            }
            return false;
        }

        private async Task<bool> ValidateGroupMember(GroupMemberModel groupMemberModel)
        {
            var groupMembers = await _unitOfWork.GroupMembers.GetAllGroupsByEntityIdAsync(groupMemberModel.EntityId ?? 0);
            if (groupMembers.Any(x => x.GroupId == groupMemberModel.GroupId))
            {
                throw new InvalidOperationException($"Member already added in this group.");
            }
            return true;
        }

        private async Task CreateEntityAndGroupToSocial(int? entityId, EntityModel entity, Group group)
        {
            var entityDetails = await _unitOfWork.Entities.GetByIdAsync(Convert.ToInt32(entityId));
            if (entityDetails.SociableProfileId == null && (entityDetails.SociableUserId == null || entityDetails.SociableUserId == -1))
            {
                if (entityDetails.PersonId != null)
                {
                    var person = await _unitOfWork.Persons.GetPersonDetailByIdAsync(Convert.ToInt32(entityDetails.PersonId));
                    var personModel = _mapper.Map<PersonModel>(person);
                    int sociableUserId = await _sociableService.CreatePerson(personModel, null, personModel.OrganizationId ?? 0);

                    if (sociableUserId > 0)
                    {
                        entity.SociableUserId = sociableUserId;
                        //Get ProfileId and update Profile

                        var userInfo = await _sociableService.GetUserById(sociableUserId, personModel.OrganizationId ?? 0);

                        dynamic profile = JObject.Parse(userInfo);
                        var profileId = profile.profile_profiles[0].target_id;
                        if (profileId > 0)
                        {

                            var result = await _sociableService.UpdatePersonProfile(personModel, null, (int)profileId, personModel.OrganizationId ?? 0);
                        }
                        entity.SociableProfileId = profileId;
                        _unitOfWork.Entities.Update(entityDetails);
                        await _unitOfWork.CommitAsync();
                    }
                }
            }

            if (entityDetails.SociableUserId != null && entityDetails.SociableProfileId != -1)
            {
                if (group.SocialGroupId == null)
                {
                    var groupModel = _mapper.Map<GroupModel>(group);
                    var socialGroupId = await _sociableGroupService.CreateSocialGroup(groupModel);
                    if (socialGroupId != 0)
                    {
                        group.SocialGroupId = socialGroupId;
                        _unitOfWork.Groups.Update(group);
                        await _unitOfWork.CommitAsync();

                        var getNewlyCreatedSocialGroupId = await _sociableGroupService.GetSocialGroup(socialGroupId, groupModel.OrganizationId ?? 0);
                        if (getNewlyCreatedSocialGroupId != null)
                        {
                            if (getNewlyCreatedSocialGroupId.id[0].value != 0)
                            {
                                var isSuccess = await _sociableGroupService.UpdateSocialGroup(groupModel, socialGroupId, true, null);
                            }
                        }
                    }
                }
            }
        }

        private async Task CreateUpdateGroupMemberToSocial(int? entityId, int? organizationId, int? socialGroupId, List<string> groupRoleList, Groupmember groupmember, bool isCreate)
        {
            if (entityId != null)
            {
                var entityDetails = await _unitOfWork.Entities.GetByIdAsync(Convert.ToInt32(entityId));

                //set social group member model
                SociableGroupMemberModel sociableGroupMemberModel = new SociableGroupMemberModel();
                SociableGroupMemberNumberValue gid = new SociableGroupMemberNumberValue
                {
                    target_id = socialGroupId
                };
                sociableGroupMemberModel.gid.Add(gid);

                SociableGroupMemberNumberValue entity = new SociableGroupMemberNumberValue
                {
                    target_id = entityDetails.SociableUserId
                };
                sociableGroupMemberModel.entity_id.Add(entity);

                foreach (var item in groupRoleList)
                {
                    SociableGroupMemberStringValue position = new SociableGroupMemberStringValue
                    {
                        target_id = item
                    };
                    sociableGroupMemberModel.field_positions.Add(position);
                }

                SociableGroupMemberStringValue type = new SociableGroupMemberStringValue
                {
                    target_id = "closed_group-group_membership"
                };
                sociableGroupMemberModel.type.Add(type);

                if (isCreate)
                {
                    // call create group member API
                    int groupMemberId = await _sociableGroupService.CreateSocialGroupMembers(sociableGroupMemberModel, Convert.ToInt32(organizationId));
                    if (groupMemberId > 0)
                    {
                        groupmember.SocialGroupMemberId = groupMemberId;
                        _unitOfWork.GroupMembers.Update(groupmember);
                        await _unitOfWork.CommitAsync();
                    }
                }
                else
                {
                    // call update group member API
                    await _sociableGroupService.UpdateSocialGroupMembers(sociableGroupMemberModel, Convert.ToInt32(organizationId), Convert.ToInt32(groupmember.SocialGroupMemberId), Convert.ToInt32(socialGroupId));
                }
            }
        }
    }
}
