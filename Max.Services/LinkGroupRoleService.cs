using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
using System.Linq;
using System.Security.Cryptography;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using Max.Data.Interfaces;
using System;
using Microsoft.AspNetCore.Http;

namespace Max.Services
{
    public class LinkGroupRoleService : ILinkGroupRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISociableGroupService _sociableGroupService;
        public LinkGroupRoleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ISociableGroupService sociableGroupService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper; ;
            _httpContextAccessor = httpContextAccessor;
            this._sociableGroupService = sociableGroupService;
        }
        public async Task<bool> CreateLinkGroupForDefaultRolesOnOrganizationSetUp(int organizationId)
        {
            var list = await _unitOfWork.GroupRoles.GetDefaultGroupRolesAsync(0);
            List<Linkgrouprole> linkgrouproleList = new List<Linkgrouprole>();

            foreach (var role in list)
            {
                Linkgrouprole linkgrouprole = new Linkgrouprole();
                linkgrouprole.GroupRoleId = role.GroupRoleId;
                linkgrouprole.IsLinked = 1;
                linkgrouprole.OrganizationId = organizationId;
                linkgrouprole.GroupRoleName = role.GroupRoleName;

                linkgrouproleList.Add(linkgrouprole);
            }

            try
            {
                await _unitOfWork.LinkGroupRoles.AddRangeAsync((IEnumerable<Linkgrouprole>)linkgrouproleList);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                throw new InvalidOperationException("Failed to add role records for organization..");
            }


            return true;
        }

        public async Task<List<LinkGroupRoleModel>> GetLinkGroupRoleByGroupId(int groupId, int status = 2)
        {
            var list = await _unitOfWork.LinkGroupRoles.GetLinkedRolesByGroupIdIdAsync(groupId);
            List<LinkGroupRoleModel> linkgrouproleList = new List<LinkGroupRoleModel>();

            foreach (var role in list)
            {
                LinkGroupRoleModel linkgrouprole = new LinkGroupRoleModel();
                linkgrouprole.LinkGroupRoleId = role.LinkGroupRoleId;
                linkgrouprole.GroupRoleId = role.GroupRoleId;
                linkgrouprole.GroupId = role.GroupId;
                linkgrouprole.IsLinked = role.GroupRoleName == "Member" ? 1 : role.IsLinked;
                linkgrouprole.OrganizationId = role.OrganizationId;
                linkgrouprole.GroupRoleName = role.GroupRole.GroupRoleName;
                linkgrouprole.IsDefault = role.GroupRole.OrganizationId == 0 ? 1 : 0;

                linkgrouproleList.Add(linkgrouprole);
            }

            if (status == 1 || status == 0)
            {
                return linkgrouproleList.Where(x => x.IsLinked == status).ToList();
            }
            else
            {
                return linkgrouproleList;
            }
        }

        public async Task<List<LinkGroupRoleModel>> GetLinkGroupRoleByOrganizationId(int organizationId)
        {
            var list = await _unitOfWork.LinkGroupRoles.GetLinkedRolesByOrganizationIdAsync(organizationId);
            List<LinkGroupRoleModel> linkgrouproleList = new List<LinkGroupRoleModel>();

            foreach (var role in list)
            {
                LinkGroupRoleModel linkgrouprole = new LinkGroupRoleModel();
                linkgrouprole.LinkGroupRoleId = role.LinkGroupRoleId;
                linkgrouprole.GroupRoleId = role.GroupRoleId;
                linkgrouprole.GroupId = role.GroupId;
                linkgrouprole.IsLinked = role.GroupRoleName == "Member" ? 1 : role.IsLinked;
                linkgrouprole.OrganizationId = role.OrganizationId;
                linkgrouprole.GroupRoleName = role.GroupRole.GroupRoleName;
                linkgrouprole.IsDefault = role.GroupRole.OrganizationId == 0 ? 1 : 0;

                linkgrouproleList.Add(linkgrouprole);
            }
            return linkgrouproleList.OrderBy(x => x.OrganizationId).ToList();
        }

        public async Task<List<LinkGroupRoleModel>> GetLinkedRolesByGroupId(int groupId, int organizationId)
        {
            var linkedRolelist = await _unitOfWork.LinkGroupRoles.GetLinkedRolesByGroupIdIdAsync(groupId);
            List<LinkGroupRoleModel> linkgrouproleList = new List<LinkGroupRoleModel>();

            foreach (var role in linkedRolelist)
            {
                LinkGroupRoleModel linkgrouprole = new LinkGroupRoleModel();
                linkgrouprole.LinkGroupRoleId = role.LinkGroupRoleId;
                linkgrouprole.GroupRoleId = role.GroupRoleId;
                linkgrouprole.GroupId = role.GroupId;
                linkgrouprole.IsLinked = role.GroupRoleName == "Member" ? 1 : role.IsLinked;
                linkgrouprole.OrganizationId = role.OrganizationId;
                linkgrouprole.GroupRoleName = role.GroupRole.GroupRoleName;
                linkgrouprole.IsDefault = role.GroupRole.OrganizationId == 0 ? 1 : 0;

                linkgrouproleList.Add(linkgrouprole);
            }


            var defaultRoleList = await _unitOfWork.GroupRoles.GetDefaultGroupRolesAsync(organizationId);

            foreach (var defaultRole in defaultRoleList)
            {
                if (!linkgrouproleList.Any(x => x.GroupRoleId == defaultRole.GroupRoleId))
                {
                    LinkGroupRoleModel linkgrouprole = new LinkGroupRoleModel();
                    linkgrouprole.LinkGroupRoleId = 0;
                    linkgrouprole.GroupRoleId = defaultRole.GroupRoleId;
                    linkgrouprole.GroupId = 0;
                    linkgrouprole.IsLinked = defaultRole.GroupRoleName == "Member" ? 1 : 0;
                    linkgrouprole.OrganizationId = organizationId;
                    linkgrouprole.GroupRoleName = defaultRole.GroupRoleName;
                    linkgrouprole.IsDefault = defaultRole.OrganizationId == 0 ? 1 : 0;

                    linkgrouproleList.Add(linkgrouprole);
                }

            }

            return linkgrouproleList;

        }

        public async Task<bool> UpdateLinkGroupRole(LinkGroupRoleModel linkGroupRoleModel)
        {
            Linkgrouprole linkgrouprole = await _unitOfWork.LinkGroupRoles.GetByIdAsync(linkGroupRoleModel.LinkGroupRoleId);
            var group = await _unitOfWork.Groups.GetByIdAsync(linkGroupRoleModel.GroupId ?? 0);

            if (linkgrouprole != null)
            {
                linkgrouprole.GroupRoleName = linkGroupRoleModel.GroupRoleName;
                linkgrouprole.IsLinked = linkGroupRoleModel.IsLinked;
                _unitOfWork.LinkGroupRoles.Update(linkgrouprole);

                var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];

                Grouphistory grouphistory = new Grouphistory();
                grouphistory.GroupId = linkGroupRoleModel.GroupId;
                grouphistory.LinkGroupRoleId = linkGroupRoleModel.LinkGroupRoleId;
                grouphistory.ActivityDate = DateTime.Now;
                grouphistory.ActivityType = "Modified";
                grouphistory.ActivityStaffId = staff.UserId;

                if (linkGroupRoleModel.IsLinked == 1)
                {
                    grouphistory.ActivityDescription = $"Activated role '{linkGroupRoleModel.GroupRoleName}' for group '{group.GroupName}'.";
                }
                else
                {
                    grouphistory.ActivityDescription = $"Inactivated role '{linkGroupRoleModel.GroupRoleName}' for group '{group.GroupName}'.";
                }

                await _unitOfWork.GroupHistories.AddAsync(grouphistory);
                await _unitOfWork.CommitAsync();

                if (group.Sync == (int)Status.Active)
                {
                    if (group.SocialGroupId != null && group.SocialGroupId != 0)
                    {
                        var isSuccess = await _sociableGroupService.UpdateSocialGroup(null, Convert.ToInt32(group.SocialGroupId), false, linkGroupRoleModel);
                    }
                    else
                    {
                        var groupModel = _mapper.Map<GroupModel>(group);
                        var socialGroupId = await _sociableGroupService.CreateSocialGroup(groupModel);
                        if (socialGroupId != 0)
                        {
                            group.SocialGroupId = socialGroupId;
                            _unitOfWork.Groups.Update(group);
                            await _unitOfWork.CommitAsync();
                            var isSuccess = await _sociableGroupService.UpdateSocialGroup(null, Convert.ToInt32(group.SocialGroupId), false, linkGroupRoleModel);
                        }
                    }
                }

                return true;
            }
            return false;
        }

        public async Task<Linkgrouprole> AddLinkGroupRole(LinkGroupRoleModel linkGroupRoleModel)
        {
            var group = await _unitOfWork.Groups.GetByIdAsync(linkGroupRoleModel.GroupId ?? 0);

            Linkgrouprole linkgrouprole = new Linkgrouprole();
            linkgrouprole.GroupRoleName = linkGroupRoleModel.GroupRoleName;
            linkgrouprole.GroupRoleId = linkGroupRoleModel.GroupRoleId;
            linkgrouprole.OrganizationId = linkGroupRoleModel.OrganizationId;
            linkgrouprole.IsLinked = linkGroupRoleModel.IsLinked;
            linkgrouprole.GroupId = linkGroupRoleModel.GroupId;

            var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];

            await _unitOfWork.LinkGroupRoles.AddAsync(linkgrouprole);

            Grouphistory grouphistory = new Grouphistory();
            grouphistory.GroupId = linkGroupRoleModel.GroupId;
            grouphistory.LinkGroupRoleId = linkGroupRoleModel.LinkGroupRoleId;
            grouphistory.ActivityDate = DateTime.Now;
            grouphistory.ActivityType = "Modified";
            grouphistory.ActivityStaffId = staff.UserId;
            if (linkGroupRoleModel.IsLinked == 1)
            {
                grouphistory.ActivityDescription = $"Activated role '{linkGroupRoleModel.GroupRoleName}' for group '{group.GroupName}'.";
            }
            else
            {
                grouphistory.ActivityDescription = $"Inactivated role '{linkGroupRoleModel.GroupRoleName}' for group '{group.GroupName}'.";
            }
            await _unitOfWork.GroupHistories.AddAsync(grouphistory);
            await _unitOfWork.CommitAsync();
            return linkgrouprole;
        }


    }
}
