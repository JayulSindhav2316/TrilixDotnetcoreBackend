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
    public class AutoBillingOnHoldRepository : Repository<Autobillingonhold>, IAutoBillingOnHoldRepository
    {
        public AutoBillingOnHoldRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<Autobillingonhold> GetAutoBillingOnHoldByMembershipIdAsync(int membershipId)
        {
            var result = await membermaxContext.Autobillingonholds
                        .Where(x => x.MembershipId == membershipId)
                        .OrderByDescending(x => x.ReviewDate)
                        .FirstOrDefaultAsync();

            return result;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
