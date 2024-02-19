using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Max.Core.Models;
using Max.Core;

namespace Max.Data.Repositories
{
    public class BillingFeeRepository : Repository<Billingfee>, IBillingFeeRepository
    {
        public BillingFeeRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Billingfee>> GetBillingFeesByMembershipIdAsync(int membershipId)
        {
            return await membermaxContext.Billingfees.Where(x => x.MembershipId == membershipId).ToListAsync();
        }
        public async Task<Billingfee> GetBillingFeeByMembershipFeeIdAsync(int membershipFeeId)
        {
            return await membermaxContext.Billingfees.Where(x => x.MembershipFeeId == membershipFeeId).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Billingfee>> GetBillingFeesDetailsByMembershipIdAsync(int membershipId)
        {
            return await membermaxContext.Billingfees.Where(x => x.MembershipId == membershipId)
                    .Include(x => x.MembershipFee)
                    .ToListAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
