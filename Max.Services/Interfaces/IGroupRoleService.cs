using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Core.Models;
using System.Threading.Tasks;


namespace Max.Services.Interfaces
{
    public interface IGroupRoleService
    {
        Task<List<Grouprole>> GetAllGroupRoles();
        Task<Grouprole> GetGroupRolesById(int id);
        Task<List<Grouprole>> GetGroupRolesByGroupId(int groupId);
        Task<List<LinkGroupRoleModel>> GetDefaultGroupRoles(int organizationId);
        Task<Grouprole> AddGroupRole(GroupRoleModel groupRoleModel);
        Task<bool> UpdateGroupRole(GroupRoleModel groupRolemodel);
    }
}
