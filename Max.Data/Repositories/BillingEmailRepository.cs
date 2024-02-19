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
    public class BillingEmailRepository : Repository<Billingemail>, IBillingEmailRepository
    {
        public BillingEmailRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Billingemail>> GetAllBillingEmailsAsync()
        {
            return await membermaxContext.Billingemails
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Billingemail> GetBillingEmailByIdAsync(int id)
        {
            return await membermaxContext.Billingemails
                .SingleOrDefaultAsync(m => m.BillingEmailId == id);
        }

        public async Task<Billingemail> GetBillingEmailByInvoiceIdAsync(int id)
        {
            return await membermaxContext.Billingemails.Where(m => m.InvoiceId == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Billingemail>> GetBillingEmailsByCycleIdAsync(int cycleId)
        {
            return await membermaxContext.Billingemails
                .Where(x => x.BillingCycleId == cycleId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Billingemail> GetBillingEmailByTokenAsync(string token)
        {
            return await membermaxContext.Billingemails
                .SingleOrDefaultAsync(m => m.Token == token);
        }

        public async Task<Billingemail> GetBillingEmailsByCycleIdAndInvoiceIdAsync(int cycleId, int invoiceId)
        {
            return await membermaxContext.Billingemails
               .Where(x => x.BillingCycleId == cycleId && x.InvoiceId == invoiceId)
               .AsNoTracking()
               .FirstOrDefaultAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
