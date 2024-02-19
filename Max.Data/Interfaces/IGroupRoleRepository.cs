using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IGroupRoleRepository : IRepository<Grouprole>
    {
        Task<IEnumerable<Grouprole>> GetAllGroupRolesAsync();
        Task<Grouprole> GetGroupRolesByIdAsync(int id);
        Task<Grouprole> GetGroupRoleByNameAsync(string groupRoleName);
        Task<IEnumerable<Grouprole>> GetDefaultGroupRolesAsync(int organizationId);
    }
}
