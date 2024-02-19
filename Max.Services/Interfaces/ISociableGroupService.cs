using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface ISociableGroupService
    {
        Task<int> CreateSocialGroup(GroupModel groupModel);
        Task<int> CreateSocialGroupMembers(SociableGroupMemberModel sociableGroupMemberModel, int? organizationId);
        Task<CreateSociableGroupResponseModel> GetSocialGroup(int socialGroupId, int? organizationId);
        Task<bool> UpdateSocialGroup(GroupModel groupModel, int socialGroupId, bool isUpdateGroup, LinkGroupRoleModel linkGroupRoleModel);
        Task<bool> UpdateSocialGroupMembers(SociableGroupMemberModel sociableGroupMemberModel, int? organizationId, int socialGroupMemberId, int groupId);
        Task<bool> DeleteSocialGroup(int socialGroupId, int? organizationId);
        Task<bool> DeleteSocialGroupMembers(int? organizationId, int socialGroupMemberId, int groupId);
    }
}
