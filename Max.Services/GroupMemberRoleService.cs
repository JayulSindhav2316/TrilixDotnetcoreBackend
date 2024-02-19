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
    public class GroupMemberRoleService : IGroupMemberRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupMemberRoleService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GroupMemberRoleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GroupMemberRoleService> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> DeleteGroupMemberRole(int groupMemberRoleId)
        {
            // Delete Group Member Role record
            var groupMemberRole = await _unitOfWork.GroupMemberRoles.GetByIdAsync(groupMemberRoleId);
            if (groupMemberRole != null)
            {
                _unitOfWork.GroupMemberRoles.Remove(groupMemberRole);
                await _unitOfWork.CommitAsync();
                return true;
            }
            //var groupMemberRoles = await _unitOfWork.GroupMemberRoles.GetAllGroupMemberRolesByGroupMemberIdAsync(groupMemberRole.GroupMemberId??0);

            //if (groupMemberRoles.Count() == 1)
            //{
            //    var groupMember = await _unitOfWork.GroupMembers.GetByIdAsync(groupMemberRole.GroupMemberId ?? 0);
            //    _unitOfWork.GroupMemberRoles.Remove(groupMemberRole);
            //    _unitOfWork.GroupMembers.Remove(groupMember);
            //    await _unitOfWork.CommitAsync();
            //    return true;
            //}

            //else if (groupMemberRole != null)
            //{
            //    _unitOfWork.GroupMemberRoles.Remove(groupMemberRole);
            //    await _unitOfWork.CommitAsync();
            //    return true;
            //}
            return false;
        }

        public async Task<bool> UpdateGroupMemberRole(GroupMemberRoleModel groupMemberRoleModel)
        {
            var groupMemberRoleRecord = await _unitOfWork.GroupMemberRoles.GetByIdAsync(groupMemberRoleModel.GroupMemberRoleId);

            if (groupMemberRoleRecord != null)
            {
                groupMemberRoleRecord.GroupRoleId = groupMemberRoleModel.GroupRoleId;
                groupMemberRoleRecord.StartDate = groupMemberRoleModel.StartDate;
                groupMemberRoleRecord.EndDate = groupMemberRoleModel.EndDate;
                groupMemberRoleRecord.Status = groupMemberRoleModel.Status;

                _unitOfWork.GroupMemberRoles.Update(groupMemberRoleRecord);

                var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];

                Grouphistory groupHistoryRoles = new Grouphistory();
                //groupHistoryRoles.GroupId = groupmembermodel.GroupId;
                //groupHistoryRoles.GroupMemberId = role.GroupMemberId;
                //groupHistoryRoles.GroupRoleId = role.GroupRoleId;
                groupHistoryRoles.ActivityDate = DateTime.Now;
                groupHistoryRoles.ActivityType = "Modified";
                groupHistoryRoles.ActivityStaffId = staff.UserId;
                if(groupMemberRoleModel.Status == 1)
                {
                    groupHistoryRoles.ActivityDescription = $"Activated role '{groupMemberRoleModel.GroupRole.GroupRoleName}'.";
                }
                else
                {
                    groupHistoryRoles.ActivityDescription = $"Inactivated role '{groupMemberRoleModel.GroupRole.GroupRoleName}'.";
                }

                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

        public async Task<List<GroupMemberRoleModel>> GetRolesByGroupMemberId(int groupMemberId)
        {
            var groupMemberRoles = await _unitOfWork.GroupMemberRoles.GetAllGroupMemberRolesByGroupMemberIdAsync(groupMemberId);
            List<GroupMemberRoleModel>  list = _mapper.Map<List<GroupMemberRoleModel>>(groupMemberRoles);
            return list;
        }
    }
}
