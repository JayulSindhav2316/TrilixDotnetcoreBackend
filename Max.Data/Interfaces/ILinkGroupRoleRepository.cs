using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface ILinkGroupRoleRepository : IRepository<Linkgrouprole>
    {
        Task<Linkgrouprole> GetIdAsync(int id);
        Task<IEnumerable<Linkgrouprole>> GetLinkedRolesByOrganizationIdAsync(int organizationId);
        Task<IEnumerable<Linkgrouprole>> GetDefaultGroupRolesAsync();
        Task<IEnumerable<Linkgrouprole>> GetLinkedRolesByGroupIdIdAsync(int groupId);
    }
}
