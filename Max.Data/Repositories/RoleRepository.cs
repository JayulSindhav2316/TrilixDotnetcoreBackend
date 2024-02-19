using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await membermaxContext.Roles
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetActiveRolesAsync()
        {
            return await membermaxContext.Roles
                .Where(x => x.Status == 1)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
        

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await membermaxContext.Roles
                .SingleOrDefaultAsync(m => m.RoleId == id);
        }

        public async Task<Role> GetRoleByNameAsync(string name)
        {
            return await membermaxContext.Roles
                .SingleOrDefaultAsync(m => m.Name == name);
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}