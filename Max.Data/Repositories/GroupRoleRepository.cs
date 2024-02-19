using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Max.Data.Repositories
{
    public class GroupRoleRepository : Repository<Grouprole>, IGroupRoleRepository
    {
        public GroupRoleRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Grouprole>> GetAllGroupRolesAsync()
        {
            return await membermaxContext.Grouproles
                .ToListAsync();
        }

        public async Task<Grouprole> GetGroupRolesByIdAsync(int id)
        {
            return await membermaxContext.Grouproles
                .SingleOrDefaultAsync(x => x.GroupRoleId == id);
        }

        public async Task<IEnumerable<Grouprole>> GetDefaultGroupRolesAsync(int organizationId)
        {
            return await membermaxContext.Grouproles
                .Where(x => x.OrganizationId == 0 || x.OrganizationId == organizationId)
                .OrderByDescending(x => x.GroupRoleName == "Member").ThenBy(x => x.OrganizationId).ThenBy(x => x.GroupRoleName)
                .ToListAsync();
        }

        public async Task<Grouprole> GetGroupRoleByNameAsync(string groupRoleName)
        {
            return await membermaxContext.Grouproles
                .FirstOrDefaultAsync(x => x.GroupRoleName == groupRoleName);
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
