using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
  public interface IRoleMenuService
  {
    Task<IEnumerable<RoleMenuModel>> GetMenuByRoleId(int roleId);
    Task<bool> UpdateRoleMenubyRoleId(dynamic requestObject);
    Task<bool> DeleteRoleMenuByRoleId(int roleId);
  }
}