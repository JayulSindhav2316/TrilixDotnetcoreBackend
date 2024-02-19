using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using System;
using Max.Core;

namespace Max.Data.Repositories
{
    public class BillingJobRepository : Repository<Billingjob>, IBillingJobRepository
    {
        public BillingJobRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Billingjob>> GetAllBillingJobsAsync()
        {
            return await membermaxContext.Billingjobs
                .ToListAsync();
        }

        public async Task<Billingjob> GetBillingJobByIdAsync(int id)
        {
            return await membermaxContext.Billingjobs
                .SingleOrDefaultAsync(m => m.BillingJobId == id);
        }
        public async Task<Billingjob> GetBillingJobByCycleIdAsync(int id)
        {
            var billingjob = await membermaxContext.Billingjobs
                                .Where(x => x.BillingCycleId == id)
                                .SingleOrDefaultAsync();

            return billingjob;
        }

        public async Task<Billingjob> GetBillingJobByDateAsync(DateTime date)
        {
            return await membermaxContext.Billingjobs.Where(x => x.CreateDate.Date == date.Date && x.Status == (int)BillingJobStatus.Created).FirstOrDefaultAsync();
        }

        public async Task<Billingjob> GetNextBillingJobAsync()
        {
            return await membermaxContext.Billingjobs.Where(x => x.Status == (int)BillingJobStatus.Created && x.BillingCycle.CycleType==(int)BillingCycleType.Manual).FirstOrDefaultAsync();
        }

        public async Task<Billingjob> GetNextRenewalJobAsync()
        {
            return await membermaxContext.Billingjobs.Where(x => x.Status == (int)BillingJobStatus.Created && x.BillingCycle.CycleType == (int)BillingCycleType.Renewal).FirstOrDefaultAsync();
        }

        public async Task<Billingjob> GetBillingFinalizationJobByDateAsync(DateTime date)
        {
            return await membermaxContext.Billingjobs.Where(x => x.StartDate.Date == date.Date && x.Status == (int)BillingJobStatus.Running && x.BillingCycle.CycleType == (int)BillingCycleType.Manual).FirstOrDefaultAsync();
        }

        public async Task<Billingjob> GetRenewalFinalizationJobByDateAsync(DateTime date)
        {
            return await membermaxContext.Billingjobs.Where(x => x.StartDate.Date == date.Date && x.Status == (int)BillingJobStatus.Running && x.BillingCycle.CycleType == (int)BillingCycleType.Renewal).FirstOrDefaultAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
