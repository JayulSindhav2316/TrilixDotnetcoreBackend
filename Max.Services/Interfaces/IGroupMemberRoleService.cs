using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Core.Models;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IGroupMemberRoleService
    {
        Task<bool> DeleteGroupMemberRole(int groupMemberRoleId);
        Task<bool> UpdateGroupMemberRole(GroupMemberRoleModel groupMemberRoleModel);
        Task<List<GroupMemberRoleModel>> GetRolesByGroupMemberId(int groupMemberId);
    }
}
