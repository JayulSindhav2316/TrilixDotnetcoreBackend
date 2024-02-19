using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class ItemTypeRepository : Repository<Itemtype>, IItemTypeRepository
    {
        public ItemTypeRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Itemtype>> GetAllItemTypesAsync()
        {
            return await membermaxContext.Itemtypes
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
