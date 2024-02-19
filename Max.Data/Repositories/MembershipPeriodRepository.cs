using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using System;

namespace Max.Data.Repositories
{
    public class MembershipPeriodRepository : Repository<Membershipperiod>, IMembershipPeriodRepository
    {
        public MembershipPeriodRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<IEnumerable<Membershipperiod>> GetAllMembershipPeriodsAsync()
        {
            return await membermaxContext.Membershipperiods
                .ToListAsync();
        }

        public async Task<Membershipperiod> GetMembershipPeriodByIdAsync(int id)
        {
            return await membermaxContext.Membershipperiods
                .SingleOrDefaultAsync(m => m.MembershipPeriodId == id);
        }

        public async Task<DateTime> GetMembershipEndDateByIdAsync(int id, DateTime startDate)
        {
            var period = await membermaxContext.Membershipperiods
                        .SingleOrDefaultAsync(m => m.MembershipPeriodId == id);
            DateTime endDate = DateTime.Now;
            if(period != null)
            {
                switch (period.PeriodUnit)
                {
                    case "Year":
                        endDate = startDate.AddYears(period.Duration).AddDays(-1);
                        break;
                    case "Month":
                        endDate = startDate.AddMonths(period.Duration).AddDays(-1);
                        break;
                    case "Day":
                        endDate = startDate.AddDays(period.Duration).AddDays(-1);
                        break;
                }
            }
            return endDate;
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
