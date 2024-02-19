using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;


namespace Max.Data.Repositories
{
    public class LinkGroupRoleRepository : Repository<Linkgrouprole>, ILinkGroupRoleRepository
    {
        public LinkGroupRoleRepository(membermaxContext context)
            : base(context)
        { }
        public async Task<IEnumerable<Linkgrouprole>> GetLinkedRolesByOrganizationIdAsync(int organizationId)
        {
            return await membermaxContext.Linkgrouproles
               .Include(x => x.GroupRole)
               .Where(x => x.OrganizationId == organizationId)
               .ToListAsync();
        }
        public async Task<IEnumerable<Linkgrouprole>> GetDefaultGroupRolesAsync()
        {
            return await membermaxContext.Linkgrouproles
                .Where(x => x.OrganizationId == 0)
                .ToListAsync();
        }

        public async Task<Linkgrouprole> GetIdAsync(int id)
        {
            return await membermaxContext.Linkgrouproles
                .SingleOrDefaultAsync(m => m.LinkGroupRoleId == id);
        }

        public async Task<IEnumerable<Linkgrouprole>> GetLinkedRolesByGroupIdIdAsync(int groupId)
        {
            return await membermaxContext.Linkgrouproles
               .Include(x => x.GroupRole)
               .Where(x => x.GroupId == groupId)
               .OrderByDescending(x => x.IsLinked).ThenBy(x => x.GroupRoleName)
               .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
