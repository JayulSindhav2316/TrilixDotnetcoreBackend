using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    class GlAccountRepository : Repository<Glaccount>, IGlAccountRepository
    {
        public GlAccountRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Glaccount>> GetAllGlaccountsAsync()
        {
            return await membermaxContext.Glaccounts
                .Include(g => g.AccountType)
                .Include(c => c.CostCenter)
                .ToListAsync();
        }

        public async Task<Glaccount> GetGlaccountByIdAsync(int id)
        {
            return await membermaxContext.Glaccounts
                .Include(g => g.AccountType)
                .Include(c => c.CostCenter)
                .Include(d => d.Items)
                .Include(m => m.Membershipfees)
                .SingleOrDefaultAsync(m => m.GlAccountId == id);
        }

        public async Task<Glaccount> GetGlaccountByNameAsync(string name)
        {
            return await membermaxContext.Glaccounts
                .Include(g => g.AccountType)
                .Include(c => c.CostCenter)
                .Include(d => d.Items)
                .Include(m => m.Membershipfees)
                .SingleOrDefaultAsync(m => m.Name == name);
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
