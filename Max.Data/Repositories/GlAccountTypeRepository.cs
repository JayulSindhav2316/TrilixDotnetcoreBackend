using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class GlAccountTypeRepository : Repository<Glaccounttype>, IGlAccountTypeRepository
    {
        public GlAccountTypeRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Glaccounttype>> GetAllGlAccountTypesAsync()
        {
            return await membermaxContext.Glaccounttypes
                .ToListAsync();
        }

        public async Task<Glaccounttype> GetGlAccountTypeByIdAsync(int id)
        {
            return await membermaxContext.Glaccounttypes
                .SingleOrDefaultAsync(m => m.AccountId == id);
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
