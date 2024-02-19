using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class StaffRoleRepository : Repository<Staffrole>, IStaffRoleRepository
    {
        public StaffRoleRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Staffrole>> GetAllStaffRolesAsync()
        {
            return await membermaxContext.Staffroles
                .ToListAsync();
        }

        public async Task<Staffrole> GetStaffRoleByIdAsync(int id)
        {
            return await membermaxContext.Staffroles
                .SingleOrDefaultAsync(m => m.StaffRoleId == id);
        }

        public async Task<IEnumerable<Staffrole>> GetAllStaffRolesByStaffIdAsync(int  staffId )
        {
            return await membermaxContext.Staffroles
                .Where(m => m.StaffId == staffId).ToListAsync();
        }
        public async Task<IEnumerable<Staffrole>> GetAllStaffRolesByRoleIdAsync(int roleId)
        {
            return await membermaxContext.Staffroles
                .Where(m => m.RoleId == roleId).ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}