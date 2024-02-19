using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IList<RoleModel>> GetAllRoles();
        Task<Role> GetRoleById(int id);
        Task<Role> CreateRole(RoleModel roleModel);
        Task<bool> UpdateRole(RoleModel roleModel);
        Task<bool> DeleteRole(int roleId);
        Task<IList<RoleModel>> GetActiveRoles();
        Task<IList<RoleModel>> GetRolesByCompanyId(int companyId);
    }
}