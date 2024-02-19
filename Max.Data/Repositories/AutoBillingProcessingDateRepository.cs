using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace Max.Data.Repositories
{
    public class AutoBillingProcessingDateRepository : Repository<Autobillingprocessingdate>, IAutoBillingProcessingDateRepository
    {
        public AutoBillingProcessingDateRepository(membermaxContext context)
            : base(context)
        { }
        public async Task<Autobillingprocessingdate> GetAutoBillingProcessingDatesByABPDIdAsync(int abpdId)
        {
            return await membermaxContext.Autobillingprocessingdates.Where(a => a.AutoBillingProcessingDatesId == abpdId).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesAsync()
        {
            return await membermaxContext.Autobillingprocessingdates.ToListAsync();
        }
        public async Task<Autobillingprocessingdate> GetAutoBillingProcessingDatesByBillingTypeAsync(string billingType)
        {
            return await membermaxContext.Autobillingprocessingdates.Where(a => a.BillingType == billingType).FirstOrDefaultAsync();
        }
        public async Task<Autobillingprocessingdate> GetAutoBillingProcessingDatesByInvoiceTypeAsync(string invoiceType)
        {
            return await membermaxContext.Autobillingprocessingdates.Where(a => a.InvoiceType == invoiceType).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesByThroughDateAsync(DateTime throughDate)
        {
            return await membermaxContext.Autobillingprocessingdates.Where(a => a.ThroughDate == throughDate).ToListAsync();
        }
        public async Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesByEffectiveDateAsync(DateTime effectiveDate)
        {
            return await membermaxContext.Autobillingprocessingdates.Where(a => a.EffectiveDate == effectiveDate).ToListAsync();
        }
        public async Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesByStatusAsync(int status)
        {
            return await membermaxContext.Autobillingprocessingdates.Where(a => a.Status == status).ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
