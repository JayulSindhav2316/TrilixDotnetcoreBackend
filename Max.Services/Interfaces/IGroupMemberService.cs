using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Core.Models;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IGroupMemberService
    {
        Task<List<GroupMemberViewModel>> GetAllGroupMembersByGroupId(int groupId);
        Task<Groupmember> AddGroupMember(GroupMemberModel groupmembermodel);
        Task<bool> AddSocialGroupMember(AddSocialGroupMember groupmembermodel);
        Task<bool> UpdateGroupMember(GroupMemberModel groupmember);
        Task<List<GroupMemberModel>> GetAllGroupsByEntityId(int entityId);
        Task<bool> DeleteGroupMember(int groupMemberId);
        Task<List<GroupSociableModel>> GetGroupsByEntityId(int entityId);
    }
}
