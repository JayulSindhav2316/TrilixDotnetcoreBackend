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

namespace Max.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupService> _logger;
        private readonly IGroupMemberService _groupMemberService;
        private readonly ICommanService _commanService;
        private readonly ISociableGroupService _sociableGroupService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GroupService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GroupService> logger, IGroupMemberService groupMemberService, ICommanService commanService, ISociableGroupService sociableGroupService, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _logger = logger;
            _groupMemberService = groupMemberService;
            _commanService = commanService;
            _sociableGroupService = sociableGroupService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Group> CreateGroup(GroupModel groupModel)
        {
            Group group = new Group();
            var isValidgroup = await ValidateGroup(groupModel);
            if (isValidgroup)
            {

                group.GroupName = groupModel.GroupName;
                group.GroupDescription = groupModel.GroupDescription;
                group.PreferredNumbers = groupModel.PreferredNumbers;
                group.ApplyTerm = groupModel.ApplyTerm;
                group.TerrmStartDate = groupModel.TerrmStartDate;
                group.TermEndDate = groupModel.TermEndDate;
                group.OrganizationId = groupModel.OrganizationId;
                group.Status = groupModel.Status;
                group.Sync = groupModel.Sync;

                foreach (var roleModel in groupModel.Roles)
                {
                    Linkgrouprole linkgrouprole = new Linkgrouprole();
                    linkgrouprole.GroupRoleId = roleModel.GroupRoleId;
                    linkgrouprole.GroupRoleName = roleModel.GroupRoleName;
                    linkgrouprole.OrganizationId = roleModel.OrganizationId;
                    linkgrouprole.IsLinked = roleModel.IsLinked;
                    group.Linkgrouproles.Add(linkgrouprole);
                }


                try
                {
                    await _unitOfWork.Groups.AddAsync(group);
                    var userId = _commanService.GetUserIdFromContext();
                    Grouphistory grouphistory = new Grouphistory();
                    grouphistory.GroupId = group.GroupId;
                    grouphistory.ActivityDate = DateTime.Now;
                    grouphistory.ActivityType = "Created";
                    grouphistory.ActivityStaffId = userId; // ActivityStaffId could be staffId or entityId
                    grouphistory.ActivityDescription = $"Created group '{groupModel.GroupName}'.";
                    await _unitOfWork.GroupHistories.AddAsync(grouphistory);

                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }

                var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                if (groupModel.Sync == (int)Status.Active && staff != null)
                {
                    try
                    {
                        var socialGroupId = await _sociableGroupService.CreateSocialGroup(groupModel);
                        if (socialGroupId != 0)
                        {
                            var getNewlyCreatedGroup = await GetGroupById(group.GroupId);
                            if (getNewlyCreatedGroup != null)
                            {
                                getNewlyCreatedGroup.SocialGroupId = socialGroupId;
                                _unitOfWork.Groups.Update(getNewlyCreatedGroup);
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
                    catch (Exception ex)
                    {
                        _logger.LogError($"Create group: Create group failed with error {ex.Message} {ex.StackTrace}");
                        throw new Exception($"Failed to Create group:");
                    }
                }

            }
            return group;
        }

        public async Task<List<GroupModel>> GetAllGroupDetailsByOrganizationId(int organizationId)
        {
            var groupList = await _unitOfWork.Groups.GetAllGroupDetailsByOrganizationIdAsync(organizationId);
            List<GroupModel> groupModelList = _mapper.Map<List<GroupModel>>(groupList);
            return groupModelList;
        }

        public async Task<List<GroupModel>> GetAllGroupsByOrganizationId(int organizationId)
        {
            var groupList = await _unitOfWork.Groups.GetAllGroupsByOrganizationIdAsync(organizationId);
            List<GroupModel> groupModelList = _mapper.Map<List<GroupModel>>(groupList);
            foreach (var item in groupModelList)
            {
                var groupMembers = await _unitOfWork.GroupMembers.GetOnlyGroupMembersByGroupIdAsync(item.GroupId);
                if (groupMembers.Any())
                {
                    item.GroupMembers = _mapper.Map<List<GroupMemberModel>>(groupMembers);
                }
            }
            return groupModelList;
        }

        public async Task<List<GroupModel>> GetGroupsByOrganizationId(int organizationId)
        {
            var groupList = await _unitOfWork.Groups.GetAllGroupDetailsByOrganizationIdAsync(organizationId);
            List<GroupModel> groupModelList = groupList.Select(item =>
            {
                var groupModel = _mapper.Map<GroupModel>(item);
                groupModel.GroupMembersCount = item.Groupmembers.Count();
                groupModel.GroupMembers = new List<GroupMemberModel>();
                return groupModel;
            }).ToList();
            return groupModelList;
        }


        public async Task<Group> GetGroupById(int id)
        {
            return await _unitOfWork.Groups.GetGroupByIdAsync(id);
        }

        public async Task<GroupModel> GetGroupByGroupId(int groupId)
        {
            var group = await _unitOfWork.Groups.GetGroupByGroupIdAsync(groupId);
            return _mapper.Map<GroupModel>(group);
        }

        public async Task<bool> UpdateGroup(GroupModel groupModel)
        {
            var isValidgroup = await ValidateGroup(groupModel);
            if (isValidgroup)
            {
                Group group = await _unitOfWork.Groups.GetGroupByIdAsync(groupModel.GroupId);

                if (group != null)
                {
                    group.GroupName = groupModel.GroupName;
                    group.GroupDescription = groupModel.GroupDescription;
                    group.PreferredNumbers = groupModel.PreferredNumbers;
                    group.ApplyTerm = groupModel.ApplyTerm;
                    group.TerrmStartDate = groupModel.TerrmStartDate;
                    group.TermEndDate = groupModel.TermEndDate;
                    group.Status = groupModel.Status;
                    group.Sync = groupModel.Sync;

                    foreach (var roleModel in groupModel.Roles.Where(x => x.LinkGroupRoleId > 0))
                    {
                        Linkgrouprole linkGroupRole = await _unitOfWork.LinkGroupRoles.GetByIdAsync(roleModel.LinkGroupRoleId);
                        Grouprole groupRole = await _unitOfWork.GroupRoles.GetByIdAsync(roleModel.GroupRoleId ?? 0);
                        linkGroupRole.GroupRoleId = roleModel.GroupRoleId;
                        linkGroupRole.GroupRoleName = groupRole.GroupRoleName;
                        linkGroupRole.OrganizationId = roleModel.OrganizationId;
                        linkGroupRole.IsLinked = roleModel.IsLinked;
                        _unitOfWork.LinkGroupRoles.Update(linkGroupRole);
                    }
                    foreach (var roleModel in groupModel.Roles.Where(x => x.LinkGroupRoleId == 0))
                    {
                        Grouprole groupRole = await _unitOfWork.GroupRoles.GetByIdAsync(roleModel.GroupRoleId ?? 0);
                        Linkgrouprole linkGroupRole = new Linkgrouprole();
                        linkGroupRole.GroupId = groupModel.GroupId;
                        linkGroupRole.GroupRoleId = roleModel.GroupRoleId;
                        linkGroupRole.GroupRoleName = groupRole.GroupRoleName;
                        linkGroupRole.OrganizationId = roleModel.OrganizationId;
                        linkGroupRole.IsLinked = roleModel.IsLinked;
                        await _unitOfWork.LinkGroupRoles.AddAsync(linkGroupRole);
                    }
                    try
                    {
                        _unitOfWork.Groups.Update(group);

                        var userId = _commanService.GetUserIdFromContext();

                        Grouphistory Grouphistory = new Grouphistory();
                        Grouphistory.GroupId = group.GroupId;
                        Grouphistory.ActivityDate = DateTime.Now;
                        Grouphistory.ActivityType = "Modified";
                        Grouphistory.ActivityStaffId = userId; // ActivityStaffId could be staffId or entityId
                        Grouphistory.ActivityDescription = $"Modified details for group '{group.GroupName}'.";

                        await _unitOfWork.GroupHistories.AddAsync(Grouphistory);
                        await _unitOfWork.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    }

                    var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                    if (group.Sync == (int)Status.Active && staff != null)
                    {
                        if (group.SocialGroupId != null && group.SocialGroupId != 0)
                        {
                            var isSuccess = await _sociableGroupService.UpdateSocialGroup(groupModel, Convert.ToInt32(group.SocialGroupId), true, null);
                        }
                        else
                        {
                            var socialGroupId = await _sociableGroupService.CreateSocialGroup(groupModel);
                            if (socialGroupId != 0)
                            {
                                var getNewlyCreatedGroup = await GetGroupById(group.GroupId);
                                if (getNewlyCreatedGroup != null)
                                {
                                    getNewlyCreatedGroup.SocialGroupId = socialGroupId;
                                    _unitOfWork.Groups.Update(getNewlyCreatedGroup);
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

                    return true;
                }
            }


            return false;
        }

        public async Task<bool> UpdateSocialGroup(UpdateSociableGroupRequestModel groupModel)
        {
            bool isSuccess = false;
            try
            {
                Group group = await _unitOfWork.Groups.GetGroupBySocialGroupIdAsync(Convert.ToInt32(groupModel.groupId[0].value));
                if (group != null)
                {
                    group.GroupName = groupModel.field_name[0].value.ToString();
                    group.GroupDescription = groupModel.field_group_description[0].value.ToString();
                    group.PreferredNumbers = Convert.ToInt32(groupModel.field_target_size[0].value);
                    group.ApplyTerm = Convert.ToInt32(groupModel.field_term[0].value);
                    group.TerrmStartDate = Convert.ToDateTime(groupModel.field_start_date[0].value);
                    group.TermEndDate = Convert.ToDateTime(groupModel.field_end_date[0].value);
                    group.Status = Convert.ToInt32(groupModel.field_isactive[0].value);

                    var linkGroupRoles = await _unitOfWork.LinkGroupRoles.GetLinkedRolesByGroupIdIdAsync(group.GroupId);
                    if (linkGroupRoles.Any())
                    {
                        foreach (var item in linkGroupRoles)
                        {
                            if (groupModel.field_group_positions.Where(s => s.value == item.GroupRoleName).Count() > 0)
                            {
                                item.IsLinked = (int)Status.Active;
                            }
                            else
                            {
                                item.IsLinked = (int)Status.InActive;
                            }
                            _unitOfWork.LinkGroupRoles.Update(item);
                        }
                    }

                    _unitOfWork.Groups.Update(group);

                    var userId = _commanService.GetUserIdFromContext();

                    Grouphistory Grouphistory = new Grouphistory();
                    Grouphistory.GroupId = group.GroupId;
                    Grouphistory.ActivityDate = DateTime.Now;
                    Grouphistory.ActivityType = "Modified";
                    Grouphistory.ActivityStaffId = userId; // ActivityStaffId could be staffId or entityId
                    Grouphistory.ActivityDescription = $"Modified details for group '{group.GroupName}'.";

                    await _unitOfWork.GroupHistories.AddAsync(Grouphistory);
                    await _unitOfWork.CommitAsync();

                    isSuccess = true;
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                  ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return false;
            }
        }

        public async Task<bool> DeleteGroup(int groupId)
        {

            var group = await _unitOfWork.Groups.GetGroupByIdAsync(groupId);
            if (group != null)
            {
                if (group.Containeraccesses.Count() > 0 || group.Documentaccesses.Count() > 0)
                {
                    throw new InvalidOperationException($"Cannot delete this group as it has documents/directory linked.");
                }
                else if (group.Groupmembers.Count() > 0)
                {
                    throw new InvalidOperationException($"Cannot delete this group as it has members linked.");
                }
                else
                {
                    var linkedRolelist = await _unitOfWork.LinkGroupRoles.GetLinkedRolesByGroupIdIdAsync(groupId);
                    if (linkedRolelist != null)
                    {
                        _unitOfWork.LinkGroupRoles.RemoveRange(linkedRolelist);
                    }

                    _unitOfWork.Groups.Remove(group);

                    var userId = _commanService.GetUserIdFromContext();

                    Grouphistory Grouphistory = new Grouphistory();
                    Grouphistory.ActivityDate = DateTime.Now;
                    Grouphistory.ActivityType = "Deleted";
                    Grouphistory.ActivityStaffId = userId;
                    Grouphistory.ActivityDescription = $"Deleted group '{group.GroupName}' -> Group Id -> {groupId}.";

                    await _unitOfWork.GroupHistories.AddAsync(Grouphistory);
                    await _unitOfWork.CommitAsync();

                    var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                    if (group.Sync == (int)Status.Active && staff != null)
                    {
                        if (group.SocialGroupId != null && group.SocialGroupId != 0)
                        {
                            var organizationId = _commanService.GetUserIdFromContext();
                            if (organizationId != null)
                            {
                                await _sociableGroupService.DeleteSocialGroup(Convert.ToInt32(group.SocialGroupId), organizationId);
                            }
                        }
                    }

                    return true;
                }
            }
            return false;
        }

        public async Task<List<GroupModel>> GetGroupsForGroupMemberByEntityId(int entityId)
        {
            var groups = await _unitOfWork.Groups.GetAllGroupsByEntityIdAsync(entityId);
            return _mapper.Map<List<GroupModel>>(groups);
        }

        public async Task<List<GroupModel>> GetGroupsByEntityId(int entityId)
        {
            var groups = await _unitOfWork.Groups.GetGroupsByEntityIdAsync(entityId);
            return _mapper.Map<List<GroupModel>>(groups);
        }

        private async Task<bool> ValidateGroup(GroupModel groupModel)
        {
            Group group = await _unitOfWork.Groups.GetGroupByNameAsync(groupModel.GroupName);
            if (group != null)
            {
                if (group.GroupId != groupModel.GroupId)
                {
                    throw new InvalidOperationException($"Duplicate Group name.");
                }
            }
            return true;
        }
    }
}
