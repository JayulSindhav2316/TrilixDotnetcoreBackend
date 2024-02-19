using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Core.Models;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface ILinkGroupRoleService
    {
        Task<List<LinkGroupRoleModel>> GetLinkGroupRoleByOrganizationId(int organizationId);
        //Task<LinkGroupRoleModel> GetLinkGroupRoleById(int id);
        Task<bool> CreateLinkGroupForDefaultRolesOnOrganizationSetUp(int organizationId);
        Task<bool> UpdateLinkGroupRole(LinkGroupRoleModel linkGroupRoleModel);
        Task<Linkgrouprole> AddLinkGroupRole(LinkGroupRoleModel linkGroupRoleModel);
        Task<List<LinkGroupRoleModel>> GetLinkedRolesByGroupId(int groupId, int organizationId);
        Task<List<LinkGroupRoleModel>> GetLinkGroupRoleByGroupId(int groupId, int status = 2);

    }
}
