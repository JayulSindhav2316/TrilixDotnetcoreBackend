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
    public class BillingDocumentRepository : Repository<Billingdocument>, IBillingDocumentRepository
    {
        public BillingDocumentRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Billingdocument>> GetAllAutoBillingDocumentDetailsAsync()
        {
            return await membermaxContext.Billingdocuments
              .Include(x => x.Autobillingdrafts)
              .OrderByDescending(x => x.CreatedDate)
              .AsNoTracking()
              .ToListAsync();
        }
        public async Task<int> GetLastBillingDocumentIdAsync()
        {
            return await membermaxContext.Billingdocuments
                .Where(b => b.IsFinalized == 1)
                .OrderByDescending(x => x.CreatedDate)
                .Select(s => s.BillingDocumentId)
                .FirstOrDefaultAsync();
        }
        public async Task<int> GetCurrentBillingDocumentIdAsync()
        {
            return await membermaxContext.Billingdocuments
                .Where(b => b.IsFinalized == 0)
                .OrderByDescending(x => x.CreatedDate)
                .Select(s => s.BillingDocumentId)
                .FirstOrDefaultAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
