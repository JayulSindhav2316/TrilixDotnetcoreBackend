using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;
using SolrNet.Utils;

namespace Max.Data.Repositories
{
    public class BillingCycleRepository : Repository<Billingcycle>, IBillingCycleRepository
    {
        public BillingCycleRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Billingcycle>> GetAllBillingCyclesAsync()
        {
            return await membermaxContext.Billingcycles
                .Include(x => x.Paperinvoices.Where(p => p.Status != (int)PaperInvoiceStatus.Deleted))
                    .ThenInclude(x => x.Invoice)
                        .ThenInclude(x => x.Invoicedetails)
                .Include(x => x.Billingjobs)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Billingcycle> GetBillingCycleByIdAsync(int id)
        {
            return await membermaxContext.Billingcycles
                .Include(x => x.Billingbatches)
                .SingleOrDefaultAsync(m => m.BillingCycleId == id);
        }
        public async Task<Billingcycle> GetBillingCycleByNameAsync(string name)
        {
            return await membermaxContext.Billingcycles
                .Include(x => x.Billingbatches)
                .Where(m => m.CycleName == name && m.CycleType == (int)BillingCycleType.Manual)
                .FirstOrDefaultAsync();
        }
        public async Task<Billingcycle> GetBillingCycleByNameAndTypeAsync(string name, int type)
        {
            return await membermaxContext.Billingcycles
                .Include(x => x.Billingbatches)
                .Where(m => m.CycleName == name && m.CycleType == type)
                .FirstOrDefaultAsync();
        }
        public async Task<int> GetLastFinalizedBillingCycleIdAsync()
        {
            return await membermaxContext.Billingcycles
                .Where(x => x.Status == (int)BillingStatus.Finalized)
                .OrderByDescending(x => x.RunDate)
                .Select(x => x.BillingCycleId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Billingcycle>> GetPendingCyclesAsync(int cycleType)
        {
            return await membermaxContext.Billingcycles
               .Include(x => x.Billingbatches)
               .Where(x => x.Status != (int)BillingStatus.Finalized && x.CycleType == cycleType)
               .OrderByDescending(x => x.RunDate)
               .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
