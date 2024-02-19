using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Core.Models;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IGroupService
    {
        Task<List<GroupModel>> GetAllGroupDetailsByOrganizationId(int organizationId);
        Task<List<GroupModel>> GetAllGroupsByOrganizationId(int organizationId);
        Task<List<GroupModel>> GetGroupsByOrganizationId(int organizationId);
        Task<Group> GetGroupById(int id);
        Task<GroupModel> GetGroupByGroupId(int groupId);
        Task<Group> CreateGroup(GroupModel groupModel);
        Task<bool> UpdateGroup(GroupModel groupModel);
        Task<bool> UpdateSocialGroup(UpdateSociableGroupRequestModel groupModel);
        Task<bool> DeleteGroup(int groupId);
        Task<List<GroupModel>> GetGroupsForGroupMemberByEntityId(int entityId);
        Task<List<GroupModel>> GetGroupsByEntityId(int entityId);
    }
}
