using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;

namespace Max.Data.Repositories
{
    public class BillingBatchRepository : Repository<Billingbatch>, IBillingBatchRepository
    {
        public BillingBatchRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Billingbatch>> GetAllBillingBatchesAsync()
        {
            return await membermaxContext.Billingbatches
                 .Include(x => x.MembershipType)
                    .ThenInclude(x => x.CategoryNavigation)
                .Include(x => x.MembershipType)
                    .ThenInclude(x => x.PeriodNavigation)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Billingbatch> GetBillingBatchByIdAsync(int id)
        {
            return await membermaxContext.Billingbatches
                .SingleOrDefaultAsync(m => m.BillingBatchId == id);
        }

        public async Task<IEnumerable<Billingbatch>> GetAllBillingBatchesByCycleIdAsync(int cycleId)
        {
            return await membermaxContext.Billingbatches
                .Include(x => x.MembershipType)
                    .ThenInclude(x => x.CategoryNavigation)
                .Include(x => x.MembershipType)
                    .ThenInclude(x => x.PeriodNavigation)
                .Where(x => x.BatchCycleId == cycleId)
                .AsNoTracking()
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
