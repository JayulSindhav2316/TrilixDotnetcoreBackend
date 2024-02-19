using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Max.Data.Repositories
{
    public class WriteOffRepository : Repository<Writeoff>, IWriteOffRepository
    {
        public WriteOffRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Writeoff>> GetWriteOffByInvoiceDetailIdAsync(int id)
        {
            var writeOffs = await membermaxContext.Writeoffs
                        .Where(x => x.InvoiceDetailId == id)
                        .ToListAsync();

            return writeOffs;
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
