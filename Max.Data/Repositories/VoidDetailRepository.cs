using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;
using System;

namespace Max.Data.Repositories
{
    public class VoidDetailRepository : Repository<Voiddetail>, IVoidDetailRepository
    {
        public VoidDetailRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Voiddetail>> GetVoidByReceiptIdAsync(int id)
        {
            var tokens = await membermaxContext.Voiddetails
                        .Where(x => x.ReceiptId == id)
                        .ToListAsync();

            return tokens;
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
