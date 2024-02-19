using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class LookupRepository : Repository<Lookup>, ILookupRepository
    {
        public LookupRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Lookup>> GetAllLookupsAsync()
        {
            return await membermaxContext.Lookups
                .ToListAsync();
        }

        public async Task<Lookup> GetLookupByIdAsync(int id)
        {
            return await membermaxContext.Lookups
                .SingleOrDefaultAsync(m => m.LookupId == id);
        }

        public async Task<string> GetLookupValueByGroupNameAsync(string groupName)
        {
            var lookup =  await membermaxContext.Lookups
                .SingleOrDefaultAsync(m => m.Group == groupName);

            if(lookup !=  null)
            {
                return lookup.Values;
            }
            return string.Empty;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}