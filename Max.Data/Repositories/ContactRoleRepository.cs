using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Max.Core;

namespace Max.Data.Repositories
{
    public class ContactRoleRepository : Repository<Contactrole>, IContactRoleRepository
    {
        public ContactRoleRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Contactrole>> GetAllContactRolesAsync()
        {
            return await membermaxContext.Contactroles
                .ToListAsync();
        }

        public async Task<IEnumerable<Contactrole>> GetActiveContactRolesAsync()
        {
            return await membermaxContext.Contactroles
                .Where(x => x.Active == (int)Status.Active)
                .ToListAsync();
        }

        public async Task<Contactrole> GetContactRoleByIdAsync(int id)
        {
            return await membermaxContext.Contactroles
                .SingleOrDefaultAsync(x => x.ContactRoleId == id);
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
