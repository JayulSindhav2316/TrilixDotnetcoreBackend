using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Organization>> GetAllOrganizationsAsync()
        {
            return await membermaxContext.Organizations
                .Include(x => x.Accountingsetups)
                .ToListAsync();
        }

        public async Task<Organization> GetOrganizationByIdAsync(int id)
        {
            return await membermaxContext.Organizations
                .Include(x => x.Accountingsetups)
                .SingleOrDefaultAsync(m => m.OrganizationId == id);
        }
        public async Task<Organization> GetOrganizationByNameAsync(string name)
        {
            return await membermaxContext.Organizations
                .SingleOrDefaultAsync(m => m.Name == name);
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
