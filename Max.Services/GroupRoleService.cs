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
using Microsoft.AspNetCore.Http;

namespace Max.Services
{
    public class GroupRoleService : IGroupRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupRoleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Grouprole> AddGroupRole(GroupRoleModel groupRoleModel)
        {
            Grouprole grouprole = new Grouprole();

            var isValidrole = await ValidateRole(groupRoleModel);
            if (isValidrole)
            {
                grouprole.GroupRoleName = groupRoleModel.GroupRoleName;
                grouprole.OrganizationId = groupRoleModel.OrganizationId;

                var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];                

                await _unitOfWork.GroupRoles.AddAsync(grouprole);
                Grouphistory grouphistory = new Grouphistory();
                grouphistory.GroupRoleId = groupRoleModel.GroupRoleId;
                grouphistory.ActivityDate = DateTime.Now;
                grouphistory.ActivityType = "Created";
                grouphistory.ActivityStaffId = staff.UserId;
                grouphistory.ActivityDescription = $"Created new position '{groupRoleModel.GroupRoleName}'.";
                await _unitOfWork.GroupHistories.AddAsync(grouphistory);
                await _unitOfWork.CommitAsync();
            }
                
            return grouprole;
        }

        public async Task<bool> UpdateGroupRole(GroupRoleModel groupRolemodel)
        {            
            Grouprole grouprole = await _unitOfWork.GroupRoles.GetGroupRolesByIdAsync(groupRolemodel.GroupRoleId);
            
            if (grouprole != null)
            {
                if(grouprole.GroupRoleName.Trim() == groupRolemodel.GroupRoleName.Trim())
                {
                    return true;
                }
                var isValidrole = await ValidateRole(groupRolemodel);
                if (isValidrole)
                {
                    string activityDescription = $"Modified position name from '{grouprole.GroupRoleName}' to '{groupRolemodel.GroupRoleName}'.";
                    grouprole.GroupRoleName = groupRolemodel.GroupRoleName.Trim();
                    _unitOfWork.GroupRoles.Update(grouprole);

                    var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];

                    Grouphistory grouphistory = new Grouphistory();
                    grouphistory.GroupRoleId = groupRolemodel.GroupRoleId;
                    grouphistory.ActivityDate = DateTime.Now;
                    grouphistory.ActivityType = "Modified";
                    grouphistory.ActivityStaffId = staff.UserId;
                    grouphistory.ActivityDescription = activityDescription;
                    await _unitOfWork.GroupHistories.AddAsync(grouphistory);

                    await _unitOfWork.CommitAsync();
                    return true;
                }
            }
            return false;
        }

        public Task<List<Grouprole>> GetAllGroupRoles()
        {
            throw new NotImplementedException();
        }

        public Task<List<Grouprole>> GetGroupRolesByGroupId(int groupId)
        {
            throw new NotImplementedException();
        }

        public async Task<Grouprole> GetGroupRolesById(int id)
        {
            return await _unitOfWork.GroupRoles.GetByIdAsync(id);
        }

        public async Task<List<LinkGroupRoleModel>> GetDefaultGroupRoles(int organizationId)
        {
            var list = await _unitOfWork.GroupRoles.GetDefaultGroupRolesAsync(organizationId);
            List<LinkGroupRoleModel> linkgrouproleList = new List<LinkGroupRoleModel>();

            foreach (var role in list)
            {
                LinkGroupRoleModel linkgrouprolemodel = new LinkGroupRoleModel();
                linkgrouprolemodel.LinkGroupRoleId = 0;
                linkgrouprolemodel.GroupRoleId = role.GroupRoleId;
                linkgrouprolemodel.GroupId = 0;
                linkgrouprolemodel.IsLinked = role.GroupRoleName == "Member" ? 1 : 0;
                linkgrouprolemodel.OrganizationId = organizationId;
                linkgrouprolemodel.GroupRoleName = role.GroupRoleName;
                linkgrouprolemodel.IsDefault = role.OrganizationId == 0 ? 1 : 0;

                linkgrouproleList.Add(linkgrouprolemodel);
            }
            return linkgrouproleList.OrderBy(x => x.OrganizationId).ToList();
        }

        private async Task<bool> ValidateRole(GroupRoleModel groupRoleModel)
        {
            var groupRoleList = await _unitOfWork.GroupRoles.GetAllGroupRolesAsync();
            if (groupRoleList != null)
            {
                if (groupRoleList.Any(x => x.GroupRoleName == groupRoleModel.GroupRoleName.Trim() && x.GroupRoleId != groupRoleModel.GroupRoleId))
                {
                    throw new InvalidOperationException($"Duplicate Position name.");
                }
            }
            return true;
        }
    }
}
