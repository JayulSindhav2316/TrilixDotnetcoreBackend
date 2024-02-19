using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Max.Core;

namespace Max.Data.Repositories
{
    public class AutoBillingJobRepository : Repository<Autobillingjob>, IAutoBillingJobRepository
    {
        public AutoBillingJobRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<Autobillingjob> GetAutoBillingJobByIdAsync(int id)
        {
            return await membermaxContext.Autobillingjobs.Where(x => x.AutoBillingJobId == id).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Autobillingjob>> GetAllAutoBillingJobsAsync()
        {
            return await membermaxContext.Autobillingjobs.ToListAsync();
        }
        public async Task<Autobillingjob> GetAutoBillingJobByDateAsync(DateTime date)
        {
            return await membermaxContext.Autobillingjobs.Where(x => x.Create.Date == date.Date).FirstOrDefaultAsync();
        }
        public async Task<Autobillingjob> GetNextAutoBillingJobByDateAsync(DateTime date)
        {
            return await membermaxContext.Autobillingjobs.Where(x => x.Create.Date == date.Date && x.Status == (int)BillingJobStatus.Created).FirstOrDefaultAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
